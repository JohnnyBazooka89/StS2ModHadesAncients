from pathlib import Path
import json

ANCIENTS = ["Shared", "Aphrodite", "Athena", "Dionysus", "Hephaestus", "Poseidon", "Zeus"]

BASE_DIR = Path("HadesAncients")
SOURCE_LANG = "eng"
TARGET_LANGS = ["kor"]


def load_json_keys(path: Path) -> set[str]:
    with path.open("r", encoding="utf-8") as f:
        data = json.load(f)

    if not isinstance(data, dict):
        raise ValueError(f"{path} is not a JSON object/map")

    return set(data.keys())


def compare_localization_keys() -> bool:
    found_any_problem = False

    for ancient in ANCIENTS:
        source_dir = BASE_DIR / ancient / "localization" / SOURCE_LANG

        if not source_dir.exists():
            print(f"[WARN] Missing source directory: {source_dir}")
            continue

        for target_lang in TARGET_LANGS:
            target_dir = BASE_DIR / ancient / "localization" / target_lang

            if not target_dir.exists():
                print(f"[MISSING DIR] {target_dir}")
                found_any_problem = True
                continue

            for source_json in source_dir.glob("*.json"):
                target_json = target_dir / source_json.name

                if not target_json.exists():
                    print(f"[MISSING FILE] {target_json}")
                    found_any_problem = True
                    continue

                try:
                    source_keys = load_json_keys(source_json)
                    target_keys = load_json_keys(target_json)
                except Exception as e:
                    print(f"[ERROR] {e}")
                    found_any_problem = True
                    continue

                missing_keys = source_keys - target_keys
                unused_extra_keys = target_keys - source_keys

                if missing_keys:
                    found_any_problem = True
                    print(f"\n[MISSING KEYS] {target_json}")
                    print("  Present in English, missing in translation:")
                    for key in sorted(missing_keys):
                        print(f"  - {key}")

                if unused_extra_keys:
                    found_any_problem = True
                    print(f"\n[UNUSED EXTRA KEYS] {target_json}")
                    print("  Present in translation, not present in English:")
                    for key in sorted(unused_extra_keys):
                        print(f"  + {key}")

    return found_any_problem


if __name__ == "__main__":
    has_problems = compare_localization_keys()

    if not has_problems:
        langs = ", ".join(TARGET_LANGS)
        print(f"All localization files for [{langs}] have exactly the same keys as {SOURCE_LANG}.")