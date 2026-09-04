
// ChildView.h : интерфейс класса CChildView
//


#pragma once


// окно CChildView

class CChildView : public CWnd
{
// Создание
public:
	CChildView();
	int Index;

// Атрибуты
public:

// Операции
public:

// Переопределение
	protected:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);

// Реализация
public:
	virtual ~CChildView();

	// Созданные функции схемы сообщений
protected:
	afx_msg void OnPaint();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnMymenuPoint1();
	afx_msg void OnMymenuPrint();
	afx_msg void OnMymenuEllipse();
	afx_msg void OnMymenuCircle();
	afx_msg void OnMymenuPicture();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
};

