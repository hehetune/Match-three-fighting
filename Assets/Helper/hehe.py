import json
import xml.etree.ElementTree as ET

# Đọc file JSON
with open('jiuweimingren.json', 'r') as f:
    json_data = json.load(f)

# Đọc file XML
tree = ET.parse('jiuweimingren.xml')
root = tree.getroot()

# Dữ liệu skeleton và armature từ XML
skeleton_name = root.attrib['name']
armature_data = root.find('armatures').find('armature')

# Tạo dictionary cho bone và attachment
bone_data = {}
attachment_data = {}

for bone in armature_data.findall('b'):
    bone_name = bone.attrib['name']
    x = float(bone.attrib['x'])
    y = float(bone.attrib['y'])
    attachment = bone.find('d').attrib['name']
    
    bone_data[bone_name] = {'x': x, 'y': y}
    attachment_data[bone_name] = attachment

# Dữ liệu frame từ JSON
frame_data = json_data['frames']

# Tạo cấu trúc xương và gán attachment
bones = []
attachments = []

for bone_name, bone_info in bone_data.items():
    bone = {
        'name': bone_name,
        'parent': '',
        'length': 0,
        'x': bone_info['x'],
        'y': bone_info['y'],
        'rotation': 0,
        'scaleX': 1,
        'scaleY': 1,
        'shearX': 0,
        'shearY': 0
    }
    bones.append(bone)
    
    attachment = attachment_data[bone_name]
    if attachment in frame_data:
        frame_info = frame_data[attachment]
        attachments.append({
            'name': attachment,
            'path': attachment,
            'x': 0,
            'y': 0,
            'scaleX': 1,
            'scaleY': 1,
            'rotation': 0,
            'width': frame_info['sourceSize'].split(',')[0].replace('{','').replace('}',''),
            'height': frame_info['sourceSize'].split(',')[1].replace('{','').replace('}','')
        })

# Xuất dữ liệu thành định dạng JSON của Spine
spine_data = {
    'skeleton': {
        'hash': '',
        'spine': '3.8.99',
        'width': 0,
        'height': 0,
        'images': '',
        'audio': ''
    },
    'bones': bones,
    'slots': [
        {
            'name': attachment,
            'bone': bone_name,
            'attachment': attachment
        }
        for bone_name, attachment in attachment_data.items()
    ],
    'skins': {
        'default': {
            'attachments': {
                bone_name: {attachment: attachment_data[bone_name]}
                for bone_name, attachment in attachment_data.items()
            }
        }
    },
    'animations': {}
}

# Lưu lại file JSON của Spine
with open('spine_skeleton.json', 'w') as f:
    json.dump(spine_data, f, indent=4)