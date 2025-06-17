import importlib

libraries = [
    "torch", "numpy", "matplotlib", "scipy",
    "mlagents", "mlagents_envs", "gym", "PIL", "cv2", "yaml"
]

for lib in libraries:
    try:
        module = importlib.import_module(lib if lib != "PIL" else "PIL.Image")
        print(f"{lib}: ✅ Loaded")
    except ImportError:
        print(f"{lib}: ❌ Not Installed")
