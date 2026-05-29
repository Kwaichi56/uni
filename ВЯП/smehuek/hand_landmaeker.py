import os
import math
import cv2
import numpy as np
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
MODEL_PATH = os.path.join(BASE_DIR, "hand_landmarker.task")

TOOLS = ["DRAW", "RECT", "CIRCLE", "CLEAR"]
tool_index = 0

HAND_CONNECTIONS = [
    (0, 1), (1, 2), (2, 3), (3, 4),
    (0, 5), (5, 6), (6, 7), (7, 8),
    (0, 9), (9, 10), (10, 11), (11, 12),
    (0, 13), (13, 14), (14, 15), (15, 16),
    (0, 17), (17, 18), (18, 19), (19, 20),
    (5, 9), (9, 13), (13, 17)
]

def to_pixel(x, y, w, h):
    x = max(0.0, min(1.0, x))
    y = max(0.0, min(1.0, y))
    return int(x * w), int(y * h)

def distance(p1, p2):
    return math.hypot(p1[0] - p2[0], p1[1] - p2[1])

def fingers_up(points):
    up = {}

    up["thumb"] = points[4][0] < points[3][0]
    up["index"] = points[8][1] < points[6][1]
    up["middle"] = points[12][1] < points[10][1]
    up["ring"] = points[16][1] < points[14][1]
    up["pinky"] = points[20][1] < points[18][1]

    return up

with open(MODEL_PATH, "rb") as f:
    model_data = f.read()

base_options = python.BaseOptions(model_asset_buffer=model_data)

options = vision.HandLandmarkerOptions(
    base_options=base_options,
    running_mode=vision.RunningMode.VIDEO,
    num_hands=1,
    min_hand_detection_confidence=0.5,
    min_hand_presence_confidence=0.5,
    min_tracking_confidence=0.5
)

cap = cv2.VideoCapture(0)

canvas = None
prev_point = None
shape_start = None
pinch_was_active = False
select_cooldown = 0

with vision.HandLandmarker.create_from_options(options) as landmarker:
    frame_id = 0

    while cap.isOpened():
        success, frame = cap.read()
        if not success:
            print("Не удалось получить кадр с камеры")
            break

        frame = cv2.flip(frame, 1)

        if canvas is None:
            canvas = np.zeros_like(frame)

        rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb)
        result = landmarker.detect_for_video(mp_image, frame_id * 33)

        h, w, _ = frame.shape

        cv2.rectangle(frame, (0, 0), (w, 60), (40, 40, 40), -1)

        for i, name in enumerate(TOOLS):
            x1 = 20 + i * 150
            x2 = x1 + 120
            color = (0, 255, 255) if i == tool_index else (180, 180, 180)
            cv2.rectangle(frame, (x1, 10), (x2, 50), color, 2)
            cv2.putText(frame, name, (x1 + 10, 37),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.8, color, 2)

        status_text = f"MODE: {TOOLS[tool_index]}"
        cv2.putText(frame, status_text, (20, h - 20),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.8, (255, 255, 255), 2)

        if result.hand_landmarks:
            hand_landmarks = result.hand_landmarks[0]
            points = [to_pixel(lm.x, lm.y, w, h) for lm in hand_landmarks]

            for a, b in HAND_CONNECTIONS:
                cv2.line(frame, points[a], points[b], (0, 255, 0), 2)
            for x, y in points:
                cv2.circle(frame, (x, y), 4, (0, 0, 255), -1)

            thumb_tip = points[4]
            index_tip = points[8]
            middle_tip = points[12]

            finger_state = fingers_up(points)

            pinch = distance(thumb_tip, index_tip) < 35

            only_index = (
                finger_state["index"]
                and not finger_state["middle"]
                and not finger_state["ring"]
                and not finger_state["pinky"]
            )

            index_middle = (
                finger_state["index"]
                and finger_state["middle"]
                and not finger_state["ring"]
                and not finger_state["pinky"]
            )

            open_palm = (
                finger_state["index"]
                and finger_state["middle"]
                and finger_state["ring"]
                and finger_state["pinky"]
            )

            cursor = index_tip
            cv2.circle(frame, cursor, 10, (255, 0, 0), -1)

            if open_palm:
                canvas[:] = 0
                prev_point = None
                shape_start = None

            if index_middle and cursor[1] < 70:
                for i in range(len(TOOLS)):
                    x1 = 20 + i * 150
                    x2 = x1 + 120
                    if x1 <= cursor[0] <= x2:
                        tool_index = i

            current_tool = TOOLS[tool_index]

            if current_tool == "CLEAR":
                canvas[:] = 0
                tool_index = 0

            if current_tool == "DRAW":
                if only_index:
                    if prev_point is None:
                        prev_point = cursor
                    cv2.line(canvas, prev_point, cursor, (0, 255, 255), 5)
                    prev_point = cursor
                else:
                    prev_point = None

            elif current_tool in ("RECT", "CIRCLE"):
                if pinch and not pinch_was_active:
                    if shape_start is None:
                        shape_start = cursor
                    else:
                        if current_tool == "RECT":
                            cv2.rectangle(canvas, shape_start, cursor, (255, 0, 255), 3)
                        elif current_tool == "CIRCLE":
                            r = int(distance(shape_start, cursor))
                            cv2.circle(canvas, shape_start, r, (0, 255, 0), 3)
                        shape_start = None

                if shape_start is not None:
                    if current_tool == "RECT":
                        cv2.rectangle(frame, shape_start, cursor, (255, 0, 255), 2)
                    elif current_tool == "CIRCLE":
                        r = int(distance(shape_start, cursor))
                        cv2.circle(frame, shape_start, r, (0, 255, 0), 2)

            pinch_was_active = pinch
        else:
            prev_point = None
            pinch_was_active = False

        output = cv2.addWeighted(frame, 1.0, canvas, 1.0, 0)
        cv2.imshow("Air Canvas", output)

        if cv2.waitKey(1) & 0xFF == 27:
            break

        frame_id += 1

cap.release()
cv2.destroyAllWindows()