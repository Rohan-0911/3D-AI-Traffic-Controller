# 🚦 3D AI Traffic Controller

![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)
![PyTorch](https://img.shields.io/badge/PyTorch-%23EE4C2C.svg?style=for-the-badge&logo=PyTorch&logoColor=white)
![Flask](https://img.shields.io/badge/flask-%23000.svg?style=for-the-badge&logo=flask&logoColor=white)
![JavaScript](https://img.shields.io/badge/javascript-%23323330.svg?style=for-the-badge&logo=javascript&logoColor=%23F7DF1E)

An autonomous, adaptive traffic optimization simulation built with Unity 3D and Deep Reinforcement Learning. 

Traditional stoplights run on blind, static timers, causing urban congestion and delaying emergency response. This project replaces static timers with a **Deep Q-Network (DQN)** that analyzes real-time vehicle density and prioritizes high-value routing to optimize traffic flow and instantly clear paths for emergency vehicles.

## 📹 Demo
*[Insert a GIF or link to your 45-second demo video here showing the simulation running]*

---

## ✨ Key Features

* **🧠 Deep Reinforcement Learning Brain:** A PyTorch-based DQN that makes millisecond decisions based on live intersection telemetry.
* **🚑 Emergency Vehicle Override (EVO):** Instantly detects incoming ambulances and firetrucks, overriding standard AI logic to clear the intersection.
* **🛡️ Autonomous Collision Avoidance:** Vehicles are equipped with front-bumper raycast sensors to detect obstacles and independently brake to prevent pile-ups.
* **📊 Live Web Dashboard:** A local HTML/JS command center that tracks real-time system metrics, lane density, and emergency alerts.

---

## 🏗️ System Architecture

This project operates on a synchronous, low-latency tri-stack architecture to ensure zero frame-dropping between the simulation and the AI:

1. **The Environment (Unity 3D / C#):** Renders the physical intersection, manages vehicle spawning, raycast physics, and calculates lane density. 
2. **The Brain (PyTorch / Python Flask):** A local HTTP server that receives high-speed JSON state data from Unity, processes it through the trained neural network, and returns optimal light-switching actions.
3. **The Command Center (HTML / JS):** A web-based interface that taps into the data stream to visualize intersection efficiency and active overrides.

---

## 🚀 Installation & Setup

### Prerequisites
* [Unity Editor](https://unity.com/) (Recommended: 2022 LTS or newer)
* [Python 3.8+](https://www.python.org/)
* [Git LFS](https://git-lfs.github.com/) (Required for pulling 3D assets)

### 1. Clone the Repository
Because this project uses high-quality 3D assets, ensure Git LFS is installed before cloning:
```bash
git lfs install
git clone [https://github.com/Rohan-0911/3D-AI-Traffic-Controller.git](https://github.com/Rohan-0911/3D-AI-Traffic-Controller.git)
cd 3D-AI-Traffic-Controller
git lfs pull
```

2. Start the AI Server
Navigate to the backend directory, install dependencies, and run the Flask server:

Bash
cd AI-Backend
pip install torch torchvision flask numpy
python traffic_ai.py
The server will start listening on localhost:5000.

3. Run the Simulation
Open Unity Hub and add the Unity-Simulation/Traffic Light folder as a project.

Open the main scene.

Press Play in the Unity Editor. The simulation will automatically connect to the local Python server and begin routing traffic!

4. Open the Dashboard
Simply double-click Web-Dashboard/website_traffic_simulator_light_theme.html in your file explorer to open the live tracking interface in your browser.

👥 The Team
This system was built collaboratively by Team Prime. Massive thanks to the core engineers who made this possible:

Rohan * Raghav

Rahul

Pruthviraj

Pulin

📄 License
This project is open-source and available under the MIT License.
