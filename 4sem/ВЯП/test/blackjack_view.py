import pygame

W, H = 1440, 880
FPS = 60
CARD_W, CARD_H, GAP = 96, 138, 28
DEALER_Y, PLAYER_Y = 150, 510
DECK_POS = pygame.Vector2(135, H // 2 - CARD_H // 2)
TABLE = pygame.Rect(50, 48, 1340, 780)
BG = (13, 67, 44)
OUTER = (24, 104, 72)
INNER = (38, 133, 92)
GOLD = (209, 176, 88)
PANEL = (232, 221, 196)
PANEL2 = (244, 236, 216)
BTN = (231, 219, 191)
BTN_H = (245, 235, 212)
BTN_D = (186, 177, 160)
BLACK = (20, 20, 20)
WHITE = (250, 250, 250)
RED = (176, 46, 60)
BACK1 = (42, 66, 128)
BACK2 = (86, 109, 192)

class Btn:
    def __init__(self, x, y, w, h, text):
        self.r = pygame.Rect(x, y, w, h)
        self.t = text
    def hit(self, pos):
        return self.r.collidepoint(pos)

def font(size, bold=False):
    return pygame.font.SysFont('arial', size, bold=bold)

def txt(surf, text, size, color, center=None, topleft=None, bold=False):
    img = font(size, bold).render(text, True, color)
    rect = img.get_rect(center=center) if center else img.get_rect(topleft=topleft)
    surf.blit(img, rect)

def panel(surf, rect, color=PANEL, border=True):
    pygame.draw.rect(surf, (0, 0, 0), rect.move(0, 4), border_radius=16)
    pygame.draw.rect(surf, color, rect, border_radius=16)
    if border: pygame.draw.rect(surf, GOLD, rect, 2, border_radius=16)

def fit(text, max_w, size):
    while size > 14 and font(size, True).size(text)[0] > max_w: size -= 1
    return font(size, True)

def card(surf, rect, rank=None, suit=None, back=False):
    panel(surf, rect, BACK1 if back else WHITE, False)
    pygame.draw.rect(surf, GOLD if back else (210, 210, 210), rect, 3 if back else 2, border_radius=12)
    if back:
        inner = rect.inflate(-12, -12)
        pygame.draw.rect(surf, BACK2, inner, border_radius=10)
        pygame.draw.circle(surf, GOLD, rect.center, 14, 3)
        return
    color = RED if suit in ('♥', '♦') else BLACK
    txt(surf, rank, 28, color, topleft=(rect.x + 10, rect.y + 8), bold=True)
    txt(surf, suit, 24, color, topleft=(rect.x + 12, rect.y + 36))
    txt(surf, suit, 48, color, center=rect.center)
    txt(surf, rank, 24, color, center=(rect.right - 18, rect.bottom - 18), bold=True)

def pos_cards(count, y):
    start = W // 2 - ((count - 1) * (CARD_W + GAP)) // 2 - CARD_W // 2
    return [pygame.Vector2(start + i * (CARD_W + GAP), y) for i in range(count)]

def deck(surf, remaining):
    for i in range(4, -1, -1):
        card(surf, pygame.Rect(DECK_POS.x - i * 2, DECK_POS.y - i * 2, CARD_W, CARD_H), back=True)
    r = pygame.Rect(56, 260, 170, 86)
    panel(surf, r, PANEL2)
    txt(surf, 'Колода', 24, BLACK, center=(r.centerx, r.y + 26), bold=True)
    txt(surf, str(remaining), 24, BLACK, center=(r.centerx, r.y + 58), bold=True)

def button(surf, btn, enabled=True):
    color = BTN if enabled else BTN_D
    pygame.draw.rect(surf, (0, 0, 0), btn.r.move(0, 4), border_radius=15)
    pygame.draw.rect(surf, color, btn.r, border_radius=15)
    pygame.draw.rect(surf, GOLD, btn.r, 2, border_radius=15)
    img = fit(btn.t, btn.r.w - 18, 21).render(btn.t, True, BLACK)
    surf.blit(img, img.get_rect(center=btn.r.center))

def draw(surf, state, buttons, balance, bet, status, remaining, cards, player_total, dealer_total):
    surf.fill(BG)
    pygame.draw.ellipse(surf, OUTER, TABLE)
    pygame.draw.ellipse(surf, GOLD, TABLE, 6)
    inner = TABLE.inflate(-50, -50)
    pygame.draw.ellipse(surf, INNER, inner)
    pygame.draw.ellipse(surf, (25, 100, 68), inner.inflate(-130, -130), 3)
    if state == 'menu':
        h = pygame.Rect(470, 175, 500, 90)
        panel(surf, h, PANEL2)
        txt(surf, 'BLACKJACK', 62, BLACK, center=h.center, bold=True)
        button(surf, buttons['play'])
        button(surf, buttons['exit'])
        return
    deck(surf, remaining)
    for r, t, s in [
        (pygame.Rect(500, 16, 440, 62), 'BLACKJACK', 38),
        (pygame.Rect(56, 72, 280, 100), None, 0),
        (pygame.Rect(430, 360, 580, 68), None, 0),
        (pygame.Rect(590, 98, 260, 44), 'Дилер', 24),
        (pygame.Rect(590, 456, 260, 44), 'Игрок', 24),
    ]:
        panel(surf, r, PANEL2 if t else PANEL)
        if t: txt(surf, t, s, BLACK, center=r.center, bold=True)
    txt(surf, f'Баланс: {balance}', 30, BLACK, topleft=(76, 92), bold=True)
    txt(surf, f'Ставка: {bet}', 30, BLACK, topleft=(76, 128), bold=True)
    pygame.draw.circle(surf, (180, 34, 44), (1270, 116), 30)
    pygame.draw.circle(surf, PANEL2, (1270, 116), 26, 4)
    pygame.draw.circle(surf, GOLD, (1270, 116), 18)
    txt(surf, str(bet), 18, BLACK, center=(1270, 116), bold=True)
    txt(surf, status, 26, BLACK, center=(720, 394), bold=True)
    if dealer_total is not None:
        r = pygame.Rect(565, 300, 310, 44); panel(surf, r, PANEL2); txt(surf, f'Сумма дилера: {dealer_total}', 22, BLACK, center=r.center, bold=True)
    if player_total is not None:
        r = pygame.Rect(565, 658, 310, 44); panel(surf, r, PANEL2); txt(surf, f'Сумма игрока: {player_total}', 22, BLACK, center=r.center, bold=True)
    enabled = {
        'up': state == 'betting' and bet < balance,
        'down': state == 'betting' and bet > 10,
        'deal': state == 'betting' and balance >= bet and balance >= 10,
        'hit': state == 'playing',
        'stand': state == 'playing',
        'again': state == 'round_over',
    }
    for name in ['up', 'down', 'deal', 'hit', 'stand']:
        button(surf, buttons[name], enabled[name])
    if enabled['again']:
        button(surf, buttons['again'])
    for c, pos, face in cards:
        card(surf, pygame.Rect(int(pos.x), int(pos.y), CARD_W, CARD_H), c.rank, c.suit, not face)
