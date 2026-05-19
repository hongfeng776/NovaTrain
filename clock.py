import tkinter as tk
from tkinter import font
import time

class DesktopClock:
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("桌面时钟")
        self.root.geometry("400x200")
        self.root.resizable(False, False)
        self.root.configure(bg='#1a1a2e')
        
        self.always_on_top = tk.BooleanVar(value=False)
        
        self.setup_ui()
        self.update_time()
        
    def setup_ui(self):
        self.time_label = tk.Label(
            self.root,
            font=font.Font(family='Helvetica', size=48, weight='bold'),
            bg='#1a1a2e',
            fg='#00d9ff'
        )
        self.time_label.pack(pady=20)
        
        self.date_label = tk.Label(
            self.root,
            font=font.Font(family='Helvetica', size=16),
            bg='#1a1a2e',
            fg='#e0e0e0'
        )
        self.date_label.pack()
        
        control_frame = tk.Frame(self.root, bg='#1a1a2e')
        control_frame.pack(pady=15)
        
        self.top_check = tk.Checkbutton(
            control_frame,
            text="始终置顶",
            variable=self.always_on_top,
            command=self.toggle_topmost,
            bg='#1a1a2e',
            fg='#ffffff',
            selectcolor='#16213e',
            activebackground='#1a1a2e',
            activeforeground='#ffffff'
        )
        self.top_check.pack()
        
    def toggle_topmost(self):
        self.root.attributes('-topmost', self.always_on_top.get())
        
    def update_time(self):
        current_time = time.strftime('%H:%M:%S')
        current_date = time.strftime('%Y年%m月%d日 %A')
        
        self.time_label.config(text=current_time)
        self.date_label.config(text=current_date)
        
        self.root.after(1000, self.update_time)
        
    def run(self):
        self.root.mainloop()

if __name__ == "__main__":
    clock = DesktopClock()
    clock.run()
