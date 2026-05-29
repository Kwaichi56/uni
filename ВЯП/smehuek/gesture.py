import os
import cv2
import math
import time
import random
import numpy as np
import mediapipe as mp
from dataclasses import dataclass, field
from mediapipe.tasks import python
from mediapipe.tasks.python import vision

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
MODEL_PATH = os.path.join(BASE_DIR, "gesture_recognizer.task")

WANTED_GESTURES = {
    "Open_Palm",
    "Closed_Fist",
    "Pointing_Up",
    "Victory",
    "Thumb_Up",
    "ILoveYou",
    "None",
}

COLORS = {
    "ARCANE": (255, 80, 180),
    "FROST": (255, 220, 80),
    "VOID": (180, 80, 255),
    "EMBER": (80, 180, 255),
}
SPELLS = list(COLORS.keys())

HAND_CONNECTIONS = [
    (0, 1), (1, 2), (2, 3), (3, 4),
    (0, 5), (5, 6), (6, 7), (7, 8),
    (0, 9), (9, 10), (10, 11), (11, 12),
    (0, 13), (13, 14), (14, 15), (15, 16),
    (0, 17), (17, 18), (18, 19), (19, 20),
    (5, 9), (9, 13), (13, 17)
]

@dataclass
class Particle:
    x: float
    y: float
    vx: float
    vy: float
    life: float
    max_life: float
    color: tuple
    size: float
    glow: float = 1.0
    gravity: float = 0.0

    def update(self, dt):
        self.vy += self.gravity * dt
        self.x += self.vx * dt
        self.y += self.vy * dt
        self.life -= dt
        self.size *= 0.995

    def alive(self):
        return self.life > 0 and self.size > 0.4


@dataclass
class Trail:
    points: list = field(default_factory=list)
    max_len: int = 25
    color: tuple = (255, 255, 255)

    def add(self, pt):
        self.points.append(pt)
        if len(self.points) > self.max_len:
            self.points.pop(0)


@dataclass
class Shockwave:
    x: float
    y: float
    r: float = 10
    max_r: float = 220
    life: float = 0.6
    color: tuple = (255, 255, 255)

    def update(self, dt):
        self.life -= dt
        self.r += 420 * dt

    def alive(self):
        return self.life > 0 and self.r < self.max_r


@dataclass
class Beam:
    x1: int
    y1: int
    x2: int
    y2: int
    life: float
    color: tuple

    def update(self, dt):
        self.life -= dt

    def alive(self):
        return self.life > 0


@dataclass
class Portal:
    x: float
    y: float
    r: float
    life: float
    color: tuple
    angle: float = 0.0

    def update(self, dt):
        self.life -= dt
        self.angle += 2.2 * dt

    def alive(self):
        return self.life > 0


class HandSmoother:
    def __init__(self, alpha=0.35):
        self.alpha = alpha
        self.prev = None

    def smooth(self, pt):
        if self.prev is None:
            self.prev = np.array(pt, dtype=np.float32)
        else:
            self.prev = self.prev * (1 - self.alpha) + np.array(pt, dtype=np.float32) * self.alpha
        return int(self.prev[0]), int(self.prev[1])


def to_pixel(x, y, w, h):
    x = max(0.0, min(1.0, x))
    y = max(0.0, min(1.0, y))
    return int(x * w), int(y * h)


def dist(a, b):
    return math.hypot(a[0] - b[0], a[1] - b[1])


def spawn_burst(particles, x, y, color, count=40, speed=260, life=(0.4, 1.0), size=(2, 6), gravity=0):
    for _ in range(count):
        ang = random.uniform(0, math.pi * 2)
        sp = random.uniform(speed * 0.4, speed)
        particles.append(
            Particle(
                x=x,
                y=y,
                vx=math.cos(ang) * sp,
                vy=math.sin(ang) * sp,
                life=random.uniform(*life),
                max_life=1.0,
                color=color,
                size=random.uniform(*size),
                glow=random.uniform(0.8, 1.5),
                gravity=gravity
            )
        )


def draw_glow_circle(img, center, radius, color, layers=4):
    x, y = center
    for i in range(layers, 0, -1):
        rr = int(radius * (1 + i * 0.6))
        alpha_color = tuple(int(c * (0.18 / i)) for c in color)
        overlay = img.copy()
        cv2.circle(overlay, (x, y), rr, alpha_color, -1, cv2.LINE_AA)
        cv2.addWeighted(overlay, 0.25, img, 0.75, 0, img)
    cv2.circle(img, (x, y), radius, color, -1, cv2.LINE_AA)


def draw_poly_glow(img, pts, color, thickness=2):
    overlay = img.copy()
    for t in [12, 8, 4]:
        cv2.polylines(overlay, [np.array(pts, dtype=np.int32)], False, color, t, cv2.LINE_AA)
        cv2.addWeighted(overlay, 0.12, img, 0.88, 0, img)
    cv2.polylines(img, [np.array(pts, dtype=np.int32)], False, color, thickness, cv2.LINE_AA)


def put_hud(frame, spell, gesture, fps, charge, combo):
    h, w = frame.shape[:2]
    overlay = frame.copy()
    cv2.rectangle(overlay, (12, 12), (420, 160), (20, 20, 20), -1)
    cv2.addWeighted(overlay, 0.45, frame, 0.55, 0, frame)

    color = COLORS[spell]
    cv2.putText(frame, "GESTURE SPELL LAB", (28, 42), cv2.FONT_HERSHEY_SIMPLEX, 1.0, (255, 255, 255), 2, cv2.LINE_AA)
    cv2.putText(frame, f"SPELL: {spell}", (28, 72), cv2.FONT_HERSHEY_SIMPLEX, 0.8, color, 2, cv2.LINE_AA)
    cv2.putText(frame, f"GESTURE: {gesture}", (28, 100), cv2.FONT_HERSHEY_SIMPLEX, 0.75, (220, 220, 220), 2, cv2.LINE_AA)
    cv2.putText(frame, f"FPS: {fps:.1f}", (28, 128), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (180, 220, 255), 2, cv2.LINE_AA)
    cv2.putText(frame, f"COMBO: x{combo}", (230, 128), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 210, 120), 2, cv2.LINE_AA)

    bx1, by1 = 28, 138
    bx2 = int(28 + 340 * min(max(charge, 0.0), 1.0))
    cv2.rectangle(frame, (bx1, by1), (370, by1 + 14), (80, 80, 80), 2)
    cv2.rectangle(frame, (bx1, by1), (bx2, by1 + 14), color, -1)


def main():
    if not os.path.exists(MODEL_PATH):
        print("Не найден файл gesture_recognizer.task рядом со скриптом")
        return

    with open(MODEL_PATH, "rb") as f:
        model_data = f.read()

    base_options = python.BaseOptions(model_asset_buffer=model_data)
    options = vision.GestureRecognizerOptions(
        base_options=base_options,
        running_mode=vision.RunningMode.VIDEO,
        num_hands=2,
        min_hand_detection_confidence=0.55,
        min_hand_presence_confidence=0.55,
        min_tracking_confidence=0.55,
    )

    cap = cv2.VideoCapture(0)
    if not cap.isOpened():
        print("Не удалось открыть камеру")
        return

    particles = []
    shockwaves = []
    beams = []
    portals = []
    trails = {"Left": Trail(max_len=30), "Right": Trail(max_len=30)}
    smoothers = {"Left": HandSmoother(0.35), "Right": HandSmoother(0.35)}

    spell_index = 0
    charge = 0.0
    combo = 1
    last_spell_switch = 0
    last_blast = 0
    last_screenshot = 0
    last_gesture = "None"
    frame_id = 0
    prev_time = time.time()
    fps = 0.0
    screen_shake = 0.0
    screenshot_counter = 1

    with vision.GestureRecognizer.create_from_options(options) as recognizer:
        while cap.isOpened():
            ok, frame = cap.read()
            if not ok:
                break

            frame = cv2.flip(frame, 1)
            h, w = frame.shape[:2]

            now = time.time()
            dt = now - prev_time
            prev_time = now
            if dt > 0:
                fps = 0.9 * fps + 0.1 * (1.0 / dt if dt > 0 else 0)

            rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb)
            result = recognizer.recognize_for_video(mp_image, frame_id * 33)

            spell = SPELLS[spell_index]
            main_color = COLORS[spell]

            overlay = np.zeros_like(frame)

            hands_info = []

            if result.hand_landmarks:
                for i, hand_landmarks in enumerate(result.hand_landmarks):
                    handedness = "Unknown"
                    if i < len(result.handedness) and len(result.handedness[i]) > 0:
                        handedness = result.handedness[i][0].category_name

                    gesture_name = "None"
                    gesture_score = 0.0
                    if i < len(result.gestures) and len(result.gestures[i]) > 0:
                        gesture_name = result.gestures[i][0].category_name
                        gesture_score = result.gestures[i][0].score

                    pts = [to_pixel(lm.x, lm.y, w, h) for lm in hand_landmarks]
                    wrist = pts[0]
                    index_tip = pts[8]
                    middle_tip = pts[12]
                    thumb_tip = pts[4]
                    palm = (
                        int((pts[0][0] + pts[5][0] + pts[9][0] + pts[13][0] + pts[17][0]) / 5),
                        int((pts[0][1] + pts[5][1] + pts[9][1] + pts[13][1] + pts[17][1]) / 5),
                    )

                    palm = smoothers["Left" if handedness == "Left" else "Right"].smooth(palm)
                    trails["Left" if handedness == "Left" else "Right"].add(palm)

                    hands_info.append({
                        "handedness": handedness,
                        "gesture": gesture_name,
                        "score": gesture_score,
                        "pts": pts,
                        "wrist": wrist,
                        "index_tip": index_tip,
                        "middle_tip": middle_tip,
                        "thumb_tip": thumb_tip,
                        "palm": palm,
                    })

                    for a, b in HAND_CONNECTIONS:
                        cv2.line(frame, pts[a], pts[b], (60, 200, 120), 1, cv2.LINE_AA)
                    for p in pts:
                        cv2.circle(frame, p, 3, (100, 120, 255), -1, cv2.LINE_AA)

            current_gesture_display = "None"

            if hands_info:
                best_hand = max(hands_info, key=lambda x: x["score"])
                current_gesture_display = best_hand["gesture"]
                last_gesture = current_gesture_display

                for hand in hands_info:
                    gesture = hand["gesture"]
                    palm = hand["palm"]
                    index_tip = hand["index_tip"]

                    draw_glow_circle(overlay, palm, 10, main_color, layers=3)

                    if gesture == "Open_Palm":
                        charge = min(1.0, charge + dt * 0.55)
                        for _ in range(5):
                            ang = random.uniform(0, math.pi * 2)
                            rr = random.uniform(30, 80)
                            px = palm[0] + math.cos(ang + now * 2) * rr
                            py = palm[1] + math.sin(ang + now * 2) * rr
                            particles.append(
                                Particle(
                                    x=px, y=py,
                                    vx=(palm[0] - px) * random.uniform(1.0, 2.0),
                                    vy=(palm[1] - py) * random.uniform(1.0, 2.0),
                                    life=random.uniform(0.3, 0.9),
                                    max_life=1.0,
                                    color=main_color,
                                    size=random.uniform(2, 5),
                                )
                            )
                        r = int(22 + 30 * charge + 10 * math.sin(now * 8))
                        cv2.circle(overlay, palm, r, main_color, 2, cv2.LINE_AA)

                    elif gesture == "Closed_Fist":
                        if charge > 0.15 and now - last_blast > 0.65:
                            blast_count = int(60 + charge * 120)
                            spawn_burst(
                                particles,
                                palm[0], palm[1],
                                main_color,
                                count=blast_count,
                                speed=420 + int(charge * 280),
                                life=(0.45, 1.25),
                                size=(2, 7),
                                gravity=20
                            )
                            shockwaves.append(Shockwave(palm[0], palm[1], r=10, max_r=260 + charge * 180, life=0.7, color=main_color))
                            charge = 0.0
                            combo = min(combo + 1, 99)
                            last_blast = now
                            screen_shake = min(18, screen_shake + 10)

                    elif gesture == "Pointing_Up":
                        beam_end = (index_tip[0], max(0, index_tip[1] - 260))
                        beams.append(Beam(index_tip[0], index_tip[1], beam_end[0], beam_end[1], 0.08, main_color))
                        spawn_burst(particles, index_tip[0], index_tip[1], main_color, count=4, speed=120, life=(0.15, 0.35), size=(1, 3))
                        charge = max(0.0, charge - dt * 0.15)

                    elif gesture == "Victory":
                        if now - last_spell_switch > 0.8:
                            spell_index = (spell_index + 1) % len(SPELLS)
                            spell = SPELLS[spell_index]
                            main_color = COLORS[spell]
                            spawn_burst(particles, palm[0], palm[1], main_color, count=55, speed=260, life=(0.3, 0.9), size=(2, 6))
                            last_spell_switch = now

                    elif gesture == "Thumb_Up":
                        if now - last_screenshot > 1.2:
                            filename = f"screenshot_magic_{screenshot_counter}.png"
                            cv2.imwrite(filename, frame)
                            screenshot_counter += 1
                            last_screenshot = now
                            spawn_burst(particles, palm[0], palm[1], (120, 255, 120), count=45, speed=220, life=(0.3, 0.8), size=(2, 5))

                    elif gesture == "ILoveYou":
                        for _ in range(6):
                            ang = random.uniform(0, math.pi * 2)
                            rr = random.uniform(15, 55)
                            px = palm[0] + math.cos(ang + now * 3.5) * rr
                            py = palm[1] + math.sin(ang + now * 3.5) * rr
                            draw_glow_circle(overlay, (int(px), int(py)), 3, (255, 255, 255), layers=2)
                        if random.random() < 0.25:
                            spawn_burst(particles, palm[0], palm[1], (255, 255, 255), count=10, speed=140, life=(0.2, 0.4), size=(1, 3))

                if len(hands_info) >= 2:
                    p1 = hands_info[0]["palm"]
                    p2 = hands_info[1]["palm"]
                    d = dist(p1, p2)

                    if d < 180:
                        center = ((p1[0] + p2[0]) // 2, (p1[1] + p2[1]) // 2)
                        portals.append(Portal(center[0], center[1], r=max(25, int(d / 2)), life=0.09, color=main_color))
                        for _ in range(5):
                            ang = random.uniform(0, math.pi * 2)
                            rr = random.uniform(10, max(20, d / 2))
                            px = center[0] + math.cos(ang) * rr
                            py = center[1] + math.sin(ang) * rr
                            particles.append(
                                Particle(
                                    x=px, y=py,
                                    vx=(center[0] - px) * random.uniform(0.8, 1.6),
                                    vy=(center[1] - py) * random.uniform(0.8, 1.6),
                                    life=random.uniform(0.25, 0.6),
                                    max_life=1.0,
                                    color=main_color,
                                    size=random.uniform(2, 4),
                                )
                            )
                        cv2.line(overlay, p1, p2, main_color, 2, cv2.LINE_AA)

            else:
                charge = max(0.0, charge - dt * 0.2)
                combo = max(1, combo - (1 if random.random() < 0.03 else 0))

            for side, trail in trails.items():
                if len(trail.points) >= 2:
                    draw_poly_glow(overlay, trail.points, main_color, thickness=2)

            for p in particles[:]:
                p.update(dt)
                if not p.alive():
                    particles.remove(p)
                    continue
                alpha = max(0.0, min(1.0, p.life / max(p.max_life, 0.0001)))
                radius = max(1, int(p.size))
                color = tuple(int(c * alpha) for c in p.color)
                draw_glow_circle(overlay, (int(p.x), int(p.y)), radius, color, layers=2)

            for s in shockwaves[:]:
                s.update(dt)
                if not s.alive():
                    shockwaves.remove(s)
                    continue
                cv2.circle(overlay, (int(s.x), int(s.y)), int(s.r), s.color, 2, cv2.LINE_AA)

            for b in beams[:]:
                b.update(dt)
                if not b.alive():
                    beams.remove(b)
                    continue
                overlay2 = overlay.copy()
                cv2.line(overlay2, (b.x1, b.y1), (b.x2, b.y2), b.color, 10, cv2.LINE_AA)
                cv2.addWeighted(overlay2, 0.16, overlay, 0.84, 0, overlay)
                cv2.line(overlay, (b.x1, b.y1), (b.x2, b.y2), (255, 255, 255), 2, cv2.LINE_AA)

            for portal in portals[:]:
                portal.update(dt)
                if not portal.alive():
                    portals.remove(portal)
                    continue
                cx, cy = int(portal.x), int(portal.y)
                r = int(portal.r)
                cv2.ellipse(overlay, (cx, cy), (r, int(r * 0.6)), math.degrees(portal.angle), 0, 360, portal.color, 2, cv2.LINE_AA)
                cv2.ellipse(overlay, (cx, cy), (int(r * 0.6), r), -math.degrees(portal.angle), 0, 360, (255, 255, 255), 1, cv2.LINE_AA)

            frame = cv2.addWeighted(frame, 1.0, overlay, 0.85, 0)

            vignette = np.zeros((h, w), dtype=np.float32)
            cv2.circle(vignette, (w // 2, h // 2), int(min(w, h) * 0.55), 1.0, -1, cv2.LINE_AA)
            vignette = cv2.GaussianBlur(vignette, (0, 0), sigmaX=90, sigmaY=90)
            vignette = np.clip(vignette, 0.35, 1.0)
            frame = (frame.astype(np.float32) * vignette[..., None]).astype(np.uint8)

            if screen_shake > 0.1:
                dx = int(random.uniform(-screen_shake, screen_shake))
                dy = int(random.uniform(-screen_shake, screen_shake))
                M = np.float32([[1, 0, dx], [0, 1, dy]])
                frame = cv2.warpAffine(frame, M, (w, h))
                screen_shake *= 0.88

            put_hud(frame, SPELLS[spell_index], current_gesture_display, fps, charge, combo)

            cv2.putText(frame, "Open_Palm=charge  Closed_Fist=blast  Pointing_Up=beam  Victory=switch  Thumb_Up=shot  ILoveYou=super",
                        (18, h - 22), cv2.FONT_HERSHEY_SIMPLEX, 0.55, (230, 230, 230), 1, cv2.LINE_AA)

            cv2.imshow("Gesture Spell Lab", frame)

            key = cv2.waitKey(1) & 0xFF
            if key == 27:
                break
            elif key == ord('1'):
                spell_index = 0
            elif key == ord('2'):
                spell_index = 1
            elif key == ord('3'):
                spell_index = 2
            elif key == ord('4'):
                spell_index = 3
            elif key == ord('c'):
                particles.clear()
                shockwaves.clear()
                beams.clear()
                portals.clear()
                trails = {"Left": Trail(max_len=30), "Right": Trail(max_len=30)}
                charge = 0.0

            frame_id += 1

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()