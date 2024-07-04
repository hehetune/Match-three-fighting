import plistlib
import json

# Đọc file .plist
with open('jiuweimingren.plist', 'rb') as f:
    plist_data = plistlib.load(f)

# Chuyển đổi sang định dạng JSON
json_data = json.dumps(plist_data, indent=4)

# Ghi ra file .json
with open('jiuweimingren.json', 'w') as f:
    f.write(json_data)