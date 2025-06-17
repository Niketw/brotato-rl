# 🧠 Brotato-RL: A Brotato-Inspired Game with Reinforcement Learning

Welcome to **Brotato-RL**, a Unity-based wave-survival game inspired by the mechanics of *Brotato*, enhanced with a Reinforcement Learning agent that learns to survive hordes of enemies using **Unity ML-Agents**.

<!--<p align="center">
  <img src="assets/gameplay_preview.gif" alt="Gameplay Preview" width="600"/>
</p>-->

---

## 🎮 Game Features

- 🔫 Fast-paced, top-down shooter combat  
- 🧟 5 enemy waves with increasing difficulty
- 🎲 Procedural wave generation for replayability

---

## 🧠 AI Features

- 🤖 Custom-trained RL agent using [Unity ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents)  
- 🧭 Observation-based learning (position, health, nearby enemies, pickups)  
- 🎯 Trained using **Proximal Policy Optimization (PPO)**  
- 📈 Training monitored via TensorBoard  

---

## 🗂️ Project Structure

```text
Brotato-RL/
├── Assets/
│   ├── Scripts/              # Game and AI agent logic
│   ├── Prefabs/              # Enemies, player, projectiles
│   └── Scenes/               # Main game scene
├── ML-Agents/                # Config and training setup
├── Models/                   # Trained ONNX/PPO models
├── Python/                   # Python training/inference scripts
├── README.md
└── requirements.txt          # Python dependencies
```

---

## 🚀 Getting Started

### Prerequisites

- Unity 2022.3 LTS or newer  
- Python 3.8+  
- [ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents) (v20+)  

### Installation

1. **Clone the repository**

```bash
git clone https://github.com/your-username/brotato-rl.git
cd brotato-rl
```

2. **Install Python dependencies**

```bash
pip install -r requirements.txt
```

3. **Open the project in Unity**

4. **Train the agent**

## 📦 Inference

Once trained, the model is automatically saved and can be used in Unity via the **Behavior Parameters** component.  
Set the behavior type to `Inference Only` and assign the `.onnx` model from the `Models/` directory.

---

<!--## 📸 Screenshots

<p float="left">
  <img src="assets/screenshot_1.png" width="45%"/>
  <img src="assets/screenshot_2.png" width="45%"/>
</p>

--- -->

## 🤝 Contributing

Contributions, ideas, and feedback are welcome!  
Feel free to fork this repo and create a pull request.

---

## 📄 License

This project is licensed under the MIT License. See `LICENSE` for details.

---

## ✨ Credits

- Inspired by **Brotato** by Blobfish  
- Built with Unity, ML-Agents, and Python  
