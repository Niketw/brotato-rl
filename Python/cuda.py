import torch

# pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu121

print("PyTorch version:", torch.__version__)

if torch.cuda.is_available():
    device = torch.device("cuda")
    print("✅ CUDA is available. Using GPU:", torch.cuda.get_device_name(device))
else:
    device = torch.device("cpu")
    print("❌ CUDA is not available. Using CPU.")

# Optional: do a small tensor operation
x = torch.rand(3, 3).to(device)
print("Tensor on device:", x.device)
