import json
import re
import sys

DECL_RE = re.compile(
    r"(?:public|private|protected|internal|static|virtual|override|abstract|sealed|async|new|readonly)"
    r"[\w\s<>\[\],.?]*?\s+(\w+)\s*(?:<[^>]*>)?\s*\(([^)]*)\)"
)


def split_params(param_text):
    depth = 0
    current = ""
    parts = []
    for ch in param_text:
        if ch in "<([":
            depth += 1
        elif ch in ">)]":
            depth -= 1
        if ch == "," and depth == 0:
            parts.append(current.strip())
            current = ""
        else:
            current += ch
    if current.strip():
        parts.append(current.strip())
    return parts


def param_type(param):
    # "Type name" or "Type name = default" -> "Type"
    param = param.split("=")[0].strip()
    tokens = param.rsplit(None, 1)
    return tokens[0] if len(tokens) == 2 else param


def find_decl(text):
    for line in text.splitlines():
        m = DECL_RE.search(line)
        if m:
            return m.group(1), split_params(m.group(2))
    return None


def is_breaking_change(old_decl, new_decl):
    old_name, old_params = old_decl
    new_name, new_params = new_decl

    if old_name != new_name:
        return True, "method name changed"

    if len(new_params) < len(old_params):
        return True, "parameter(s) removed"

    for i, old_p in enumerate(old_params):
        if param_type(old_p) != param_type(new_params[i]):
            return True, f"parameter {i} type changed"

    for extra in new_params[len(old_params):]:
        if "=" not in extra:
            return True, "new required parameter added"

    return False, None


data = json.load(sys.stdin)
tool_input = data.get("tool_input", {}) or {}
path = tool_input.get("file_path", "")
old_string = tool_input.get("old_string", "")
new_string = tool_input.get("new_string", "")

if path.endswith(".cs") and old_string and new_string:
    old_decl = find_decl(old_string)
    new_decl = find_decl(new_string)

    if old_decl and new_decl:
        breaking, reason = is_breaking_change(old_decl, new_decl)
        if breaking:
            msg = (
                f"Reminder: this edit to {path} looks like a breaking signature change on "
                f"'{old_decl[0]}' ({reason}). Per the refactoring protocol, grep the whole repo for "
                "all call sites and update them in the same change before considering this done."
            )
            print(json.dumps({"systemMessage": msg}))
