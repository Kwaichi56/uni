import tkinter as tk
from tkinter import filedialog, messagebox
from PIL import Image, ImageTk, ImageEnhance, ImageFilter, ImageOps
import cv2
import numpy as np

class ImageEditorApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Фоторедактор")
        self.root.geometry("1100x700")
        
        self.original_image = None
        self.current_image = None
        self.display_image = None
        
        self.setup_menu()
        self.setup_ui()
        
    def setup_menu(self):
        menubar = tk.Menu(self.root)
        file_menu = tk.Menu(menubar, tearoff=0)
        file_menu.add_command(label="Open", command=self.open_image)
        file_menu.add_command(label="Save", command=self.save_image)
        file_menu.add_separator()
        file_menu.add_command(label="Exit", command=self.root.quit)
        
        menubar.add_cascade(label="File", menu=file_menu)
        self.root.config(menu=menubar)
        
    def setup_ui(self):
        self.left_frame = tk.Frame(self.root)
        self.left_frame.pack(side=tk.LEFT, fill=tk.BOTH, expand=True, padx=10, pady=10)
        
        self.image_label = tk.Label(self.left_frame, text="I M A G E", bg="lightgray", width=60, height=20)
        self.image_label.pack(fill=tk.BOTH, expand=True)
        
        self.info_label = tk.Label(self.left_frame, text="Свойства: Формат: -, Размер: -, Режим: -", font=("Arial", 10, "bold"))
        self.info_label.pack(pady=10)
        
        self.right_frame = tk.Frame(self.root, width=250)
        self.right_frame.pack(side=tk.RIGHT, fill=tk.Y, padx=10, pady=10)
        
        tk.Label(self.right_frame, text="Базовые операции (Pillow):", font=("Arial", 10, "bold")).pack(pady=5)
        
        tk.Button(self.right_frame, text="1. Rotate 90°", command=self.rotate_image, width=20).pack(pady=2)
        tk.Button(self.right_frame, text="2. Grayscale (Ч/Б)", command=self.grayscale_image, width=20).pack(pady=2)
        tk.Button(self.right_frame, text="3. Blur (Размытие)", command=self.blur_image, width=20).pack(pady=2)
        tk.Button(self.right_frame, text="4. Sharpen (Резкость)", command=self.sharpen_image, width=20).pack(pady=2)
        tk.Button(self.right_frame, text="5. Отразить по горизонтали", command=self.flip_image, width=20).pack(pady=2)
        tk.Button(self.right_frame, text="6. Инверсия цветов", command=self.invert_image, width=20).pack(pady=2)
        
        tk.Label(self.right_frame, text="7. Контрастность (параметр):").pack(pady=(10, 0))
        self.contrast_slider = tk.Scale(self.right_frame, from_=0.1, to=3.0, resolution=0.1, orient=tk.HORIZONTAL)
        self.contrast_slider.set(1.0)
        self.contrast_slider.pack()
        tk.Button(self.right_frame, text="Применить контраст", command=self.apply_contrast, width=20, bg="lightblue").pack(pady=2)
        
        
        tk.Button(self.right_frame, text="Reset (Сброс к оригиналу)", command=self.reset_image, width=20, bg="lightcoral").pack(pady=25)
        
    def open_image(self):
        file_path = filedialog.askopenfilename(filetypes=[("Image files", "*.jpg *.jpeg *.png *.bmp")])
        if file_path:
            self.original_image = Image.open(file_path)
            self.current_image = self.original_image.copy()
            
            img_format = self.original_image.format
            img_size = self.original_image.size
            img_mode = self.original_image.mode
            self.info_label.config(text=f"Свойства: Формат: {img_format}, Размер: {img_size[0]}x{img_size[1]}, Режим: {img_mode}")
            
            self.update_image_display()
            
    def save_image(self):
        if self.current_image:
            file_path = filedialog.asksaveasfilename(defaultextension=".png", filetypes=[("PNG", "*.png"), ("JPEG", "*.jpg")])
            if file_path:
                self.current_image.save(file_path)
                messagebox.showinfo("Успех", "Изображение сохранено!")

    def update_image_display(self):
        if self.current_image:
            display_img = self.current_image.copy()
            display_img.thumbnail((800, 600))
            self.display_image = ImageTk.PhotoImage(display_img)
            self.image_label.config(image=self.display_image, text="", bg="SystemButtonFace")
        
    def rotate_image(self):
        if self.current_image:
            self.current_image = self.current_image.rotate(90, expand=True)
            self.update_image_display()
            
    def grayscale_image(self):
        if self.current_image:
            self.current_image = self.current_image.convert("L")
            self.update_image_display()
            
    def blur_image(self):
        if self.current_image:
            self.current_image = self.current_image.filter(ImageFilter.BLUR)
            self.update_image_display()
            
    def sharpen_image(self):
        if self.current_image:
            self.current_image = self.current_image.filter(ImageFilter.SHARPEN)
            self.update_image_display()
            
    def flip_image(self):
        if self.current_image:
            self.current_image = self.current_image.transpose(Image.FLIP_LEFT_RIGHT)
            self.update_image_display()

    def invert_image(self):
        if self.current_image:
            if self.current_image.mode != "RGB":
                self.current_image = self.current_image.convert("RGB")
            self.current_image = ImageOps.invert(self.current_image)
            self.update_image_display()
            
    def apply_contrast(self):
        if self.current_image:
            factor = self.contrast_slider.get()
            enhancer = ImageEnhance.Contrast(self.current_image)
            self.current_image = enhancer.enhance(factor)
            self.update_image_display()
            
    def reset_image(self):
        if self.original_image:
            self.current_image = self.original_image.copy()
            self.update_image_display()
            

if __name__ == "__main__":
    root = tk.Tk()
    app = ImageEditorApp(root)
    root.mainloop()