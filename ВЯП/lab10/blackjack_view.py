import pygame

SCREEN_WIDTH = 1440
SCREEN_HEIGHT = 880
FPS = 60

TABLE_RECT = pygame.Rect(50, 48, 1340, 780)
CARD_W = 96
CARD_H = 138
CARD_GAP = 28
DEALER_Y = 150
PLAYER_Y = 510
DECK_POS = pygame.Vector2(135, SCREEN_HEIGHT // 2 - CARD_H // 2)

BG_COLOR = (13, 67, 44)
TABLE_OUTER = (24, 104, 72)
TABLE_INNER = (38, 133, 92)
GOLD = (209, 176, 88)
PANEL = (232, 221, 196)
PANEL_2 = (244, 236, 216)
BUTTON = (231, 219, 191)
BUTTON_HOVER = (245, 235, 212)
BUTTON_DISABLED = (186, 177, 160)
BLACK = (20, 20, 20)
WHITE = (250, 250, 250)
RED = (176, 46, 60)
SHADOW = (0, 0, 0, 70)
BACK_DARK = (42, 66, 128)
BACK_LIGHT = (86, 109, 192)
BACK_GOLD = (226, 194, 96)


class Button:
    def __init__(self, x, y, w, h, text):
        self.rect = pygame.Rect(x, y, w, h)
        self.text = text
        self.hovered = False
        self.enabled = True
        self.visible = True

    def contains(self, pos):
        return self.rect.collidepoint(pos)


def card_rect(pos):
    return pygame.Rect(int(pos.x), int(pos.y), CARD_W, CARD_H)


def get_font(size, bold=False):
    return pygame.font.SysFont('arial', size, bold=bold)


def draw_text(surface, text, size, color, center=None, topleft=None, bold=False):
    img = get_font(size, bold).render(text, True, color)
    rect = img.get_rect()
    if center:
        rect.center = center
    if topleft:
        rect.topleft = topleft
    surface.blit(img, rect)
    return rect


def draw_panel(surface, rect, color=PANEL, border=True):
    shadow = pygame.Surface((rect.w + 10, rect.h + 10), pygame.SRCALPHA)
    pygame.draw.rect(shadow, SHADOW, shadow.get_rect(), border_radius=18)
    surface.blit(shadow, (rect.x - 5, rect.y + 6))
    pygame.draw.rect(surface, color, rect, border_radius=18)
    if border:
        pygame.draw.rect(surface, GOLD, rect, width=2, border_radius=18)


def draw_background(surface):
    surface.fill(BG_COLOR)
    pygame.draw.ellipse(surface, TABLE_OUTER, TABLE_RECT)
    pygame.draw.ellipse(surface, GOLD, TABLE_RECT, 6)
    inner = TABLE_RECT.inflate(-50, -50)
    pygame.draw.ellipse(surface, TABLE_INNER, inner)
    pygame.draw.ellipse(surface, (25, 100, 68), inner.inflate(-130, -130), 3)


def fit_text(text, max_width, start_size, bold=True):
    size = start_size
    while size > 14:
        font = get_font(size, bold)
        if font.size(text)[0] <= max_width:
            return font
        size -= 1
    return get_font(14, bold)


def draw_button(surface, button):
    if not button.visible:
        return
    color = BUTTON_DISABLED if not button.enabled else (BUTTON_HOVER if button.hovered else BUTTON)
    shadow = button.rect.move(0, 4)
    pygame.draw.rect(surface, (0, 0, 0), shadow, border_radius=15)
    pygame.draw.rect(surface, color, button.rect, border_radius=15)
    pygame.draw.rect(surface, GOLD, button.rect, width=2, border_radius=15)
    font = fit_text(button.text, button.rect.w - 18, 21)
    img = font.render(button.text, True, BLACK)
    surface.blit(img, img.get_rect(center=button.rect.center))


def draw_chip(surface, x, y, value):
    pygame.draw.circle(surface, (180, 34, 44), (x, y), 30)
    pygame.draw.circle(surface, PANEL_2, (x, y), 26, 4)
    pygame.draw.circle(surface, GOLD, (x, y), 18)
    draw_text(surface, str(value), 18, BLACK, center=(x, y), bold=True)


def draw_card_front(surface, rect, rank, suit):
    draw_panel(surface, rect, color=WHITE, border=False)
    pygame.draw.rect(surface, (210, 210, 210), rect, 2, border_radius=12)
    color = RED if suit in ('♥', '♦') else BLACK
    draw_text(surface, rank, 28, color, topleft=(rect.x + 10, rect.y + 8), bold=True)
    draw_text(surface, suit, 24, color, topleft=(rect.x + 12, rect.y + 36))
    draw_text(surface, suit, 48, color, center=rect.center)
    draw_text(surface, rank, 24, color, center=(rect.right - 18, rect.bottom - 18), bold=True)


def draw_card_back(surface, rect):
    draw_panel(surface, rect, color=BACK_DARK, border=False)
    pygame.draw.rect(surface, BACK_GOLD, rect, 3, border_radius=12)
    inner = rect.inflate(-12, -12)
    pygame.draw.rect(surface, BACK_LIGHT, inner, border_radius=10)
    for i in range(6):
        y = inner.y + 10 + i * 18
        pygame.draw.line(surface, BACK_DARK, (inner.x + 8, y), (inner.right - 8, y), 2)
    for i in range(4):
        x = inner.x + 12 + i * 18
        pygame.draw.line(surface, BACK_DARK, (x, inner.y + 8), (x, inner.bottom - 8), 1)
    diamond = [
        (rect.centerx, rect.y + 26),
        (rect.right - 26, rect.centery),
        (rect.centerx, rect.bottom - 26),
        (rect.x + 26, rect.centery),
    ]
    pygame.draw.polygon(surface, BACK_GOLD, diamond, 3)
    pygame.draw.circle(surface, BACK_GOLD, rect.center, 14, 3)


def draw_deck(surface, remaining):
    for i in range(4, -1, -1):
        rect = pygame.Rect(DECK_POS.x - i * 2, DECK_POS.y - i * 2, CARD_W, CARD_H)
        draw_card_back(surface, rect)
    panel = pygame.Rect(56, 260, 170, 86)
    draw_panel(surface, panel, PANEL_2)
    draw_text(surface, 'Колода', 24, BLACK, center=(panel.centerx, panel.y + 26), bold=True)
    draw_text(surface, str(remaining), 24, BLACK, center=(panel.centerx, panel.y + 58), bold=True)


def draw_labels(surface, balance, bet, message, player_total, dealer_total, phase):
    title_panel = pygame.Rect(500, 16, 440, 62)
    draw_panel(surface, title_panel, PANEL_2)
    draw_text(surface, 'BLACKJACK', 38, BLACK, center=title_panel.center, bold=True)

    balance_panel = pygame.Rect(56, 72, 280, 100)
    draw_panel(surface, balance_panel)
    draw_text(surface, f'Баланс: {balance}', 30, BLACK, topleft=(76, 92), bold=True)
    draw_text(surface, f'Ставка: {bet}', 30, BLACK, topleft=(76, 128), bold=True)
    draw_chip(surface, 1270, 116, bet)

    msg_panel = pygame.Rect(430, 360, 580, 68)
    draw_panel(surface, msg_panel, PANEL_2)
    draw_text(surface, message, 26, BLACK, center=msg_panel.center, bold=True)

    top_panel = pygame.Rect(590, 98, 260, 44)
    bottom_panel = pygame.Rect(590, 456, 260, 44)
    draw_panel(surface, top_panel, PANEL_2)
    draw_panel(surface, bottom_panel, PANEL_2)
    draw_text(surface, 'Дилер', 24, BLACK, center=top_panel.center, bold=True)
    draw_text(surface, 'Игрок', 24, BLACK, center=bottom_panel.center, bold=True)

    if dealer_total is not None:
        dealer_panel = pygame.Rect(565, 300, 310, 44)
        draw_panel(surface, dealer_panel, PANEL_2)
        draw_text(surface, f'Сумма дилера: {dealer_total}', 22, BLACK, center=dealer_panel.center, bold=True)
    if player_total is not None:
        player_panel = pygame.Rect(565, 658, 310, 44)
        draw_panel(surface, player_panel, PANEL_2)
        draw_text(surface, f'Сумма игрока: {player_total}', 22, BLACK, center=player_panel.center, bold=True)


def draw_menu(surface, play_button, exit_button):
    draw_background(surface)
    header = pygame.Rect(470, 175, 500, 90)
    draw_panel(surface, header, PANEL_2)
    draw_text(surface, 'BLACKJACK', 62, BLACK, center=header.center, bold=True)
    draw_button(surface, play_button)
    draw_button(surface, exit_button)


def draw_cards(surface, cards):
    for card, pos, face_up in cards:
        rect = card_rect(pos)
        if face_up:
            draw_card_front(surface, rect, card.rank, card.suit)
        else:
            draw_card_back(surface, rect)


def draw_controls(surface, btn_hit, btn_stand, btn_bet_up, btn_bet_down, btn_deal, btn_replay):
    for button in (btn_bet_up, btn_bet_down, btn_deal, btn_hit, btn_stand, btn_replay):
        draw_button(surface, button)


def draw_scene(surface, state, menu_play, menu_exit, btn_hit, btn_stand, btn_bet_up, btn_bet_down,
               btn_deal, btn_replay, balance, bet, message, remaining, static_cards,
               animated_cards, player_total, dealer_total, phase):
    if state == 'menu':
        draw_menu(surface, menu_play, menu_exit)
        return

    draw_background(surface)
    draw_deck(surface, remaining)
    draw_labels(surface, balance, bet, message, player_total, dealer_total, phase)
    draw_controls(surface, btn_hit, btn_stand, btn_bet_up, btn_bet_down, btn_deal, btn_replay)
    draw_cards(surface, static_cards)
    draw_cards(surface, animated_cards)