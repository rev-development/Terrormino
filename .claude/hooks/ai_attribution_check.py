import json
import sys

data = json.load(sys.stdin)
tool_input = data.get("tool_input", {}) or {}
tool_response = data.get("tool_response", {}) or {}
path = tool_input.get("file_path", "") or tool_response.get("filePath", "")

if path.endswith(".cs"):
    try:
        with open(path, encoding="utf-8") as f:
            content = f.read()
    except OSError:
        sys.exit(0)

    if "AiGenerated" not in content:
        msg = (
            f"Reminder: {path} was written/edited and has no [AiGenerated] attribute anywhere in the file. "
            "If real logic was written or meaningfully modified, tag it per project convention "
            "(Helpers.AiGeneratedAttribute). Not required for pure file moves/copy-pastes with no logic change."
        )
        print(json.dumps({"systemMessage": msg}))
