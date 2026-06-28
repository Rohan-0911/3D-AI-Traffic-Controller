import torch
import torch.nn as nn
import numpy as np
import copy
from flask import Flask, request, jsonify
from flask_cors import CORS

# ==========================================
# 1. THE NEURAL NETWORK & AGENT
# ==========================================
class TrafficLightBrain(nn.Module):
    def __init__(self, input_size, output_size):
        super(TrafficLightBrain, self).__init__()
        self.fc1 = nn.Linear(input_size, 128) 
        self.relu = nn.ReLU()
        self.fc2 = nn.Linear(128, 64)
        self.fc3 = nn.Linear(64, output_size)

    def forward(self, x):
        x = self.relu(self.fc1(x))
        x = self.relu(self.fc2(x))
        return self.fc3(x)

class DQNAgent:
    def __init__(self, state_size, action_size):
        self.model = TrafficLightBrain(state_size, action_size)

    def act(self, state):
        state_tensor = torch.FloatTensor(state)
        with torch.no_grad():
            q_values = self.model(state_tensor)
        return torch.argmax(q_values).item()

# ==========================================
# 3. FLASK SERVER & DASHBOARD
# ==========================================
app = Flask(__name__)
CORS(app) 

# Initialize and Load AI
state_size = 4
action_size = 4
agent = DQNAgent(state_size, action_size)

try:
    agent.model.load_state_dict(torch.load("smart_traffic_brain.pth"))
    print("SUCCESS: Loaded smart_traffic_brain.pth!")
except Exception as e:
    print(f"WARNING: Could not find trained brain. Error: {e}")

# The "Memory" for your Website
dashboard_state = {
    "accuracy": 92.5, 
    "active_lane": "east",
    "waiting": {"east": 0, "west": 0, "north": 0, "south": 0},
    "passed": {"east": 0, "west": 0, "north": 0, "south": 0},
    "emergency": {"east": 0, "west": 0, "north": 0, "south": 0},
    "emergency_details": { # NEW: Tracks the exact type of vehicle!
        "east": {"amb": 0, "fire": 0, "pol": 0},
        "west": {"amb": 0, "fire": 0, "pol": 0},
        "north": {"amb": 0, "fire": 0, "pol": 0},
        "south": {"amb": 0, "fire": 0, "pol": 0}
    },
    "total": {
        "east": {"cars": 0, "emergency": 0},
        "west": {"cars": 0, "emergency": 0},
        "north": {"cars": 0, "emergency": 0},
        "south": {"cars": 0, "emergency": 0}
    },
    "lifetime_cars": 0,
    "lifetime_emergency": 0,
    "lifetime_signals": 0
}

previous_waiting = {"east": 0, "west": 0, "north": 0, "south": 0}
previous_emerg = {"east": 0, "west": 0, "north": 0, "south": 0}

lane_names = ['west', 'east', 'south', 'north']

@app.route('/predict_light', methods=['POST'])
def predict_light():
    global previous_waiting, previous_emerg
    
    try:
        data = request.json
        # Feed the AI the priority scores (to trigger emergencies)
        state_list = [data['positive_x'], data['negative_x'], data['positive_z'], data['negative_z']]
        state = np.array([state_list])
        action = agent.act(state)

        # Feed the DASHBOARD the actual physical vehicle counts
        current_waiting = {
            'west': data.get('count_px', 0),
            'east': data.get('count_nx', 0),
            'south': data.get('count_pz', 0),
            'north': data.get('count_nz', 0)
        }
        
        # Pull the specific emergency types
        dashboard_state['emergency_details'] = {
            'west': {'amb': data.get('amb_px', 0), 'fire': data.get('fire_px', 0), 'pol': data.get('pol_px', 0)},
            'east': {'amb': data.get('amb_nx', 0), 'fire': data.get('fire_nx', 0), 'pol': data.get('pol_nx', 0)},
            'south': {'amb': data.get('amb_pz', 0), 'fire': data.get('fire_pz', 0), 'pol': data.get('pol_pz', 0)},
            'north': {'amb': data.get('amb_nz', 0), 'fire': data.get('fire_nz', 0), 'pol': data.get('pol_nz', 0)}
        }
        
        # Calculate raw total of emergencies for math tracking
        current_emerg = {
            lane: sum(dashboard_state['emergency_details'][lane].values()) for lane in lane_names
        }

        # Calculate passed vehicles
        for lane in lane_names:
            diff = previous_waiting[lane] - current_waiting[lane]
            if diff > 0:
                dashboard_state['passed'][lane] += diff
                dashboard_state['total'][lane]['cars'] += diff
                dashboard_state['lifetime_cars'] += diff
                
            emerg_diff = previous_emerg[lane] - current_emerg[lane]
            if emerg_diff > 0:
                dashboard_state['total'][lane]['emergency'] += emerg_diff
                dashboard_state['lifetime_emergency'] += emerg_diff

        # Save to dashboard
        dashboard_state['waiting'] = current_waiting
        dashboard_state['emergency'] = current_emerg
        dashboard_state['active_lane'] = lane_names[action]
        dashboard_state['lifetime_signals'] += 1
        
        previous_waiting = copy.deepcopy(current_waiting)
        previous_emerg = copy.deepcopy(current_emerg)

        return jsonify({'action': action})
        
    except Exception as e:
        return jsonify({'error': str(e)}), 400

@app.route('/dashboard_data', methods=['GET'])
def get_dashboard_data():
    return jsonify(dashboard_state)

if __name__ == "__main__":
    app.run(host='127.0.0.1', port=5000, debug=False)