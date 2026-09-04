
// ChildView.cpp : реализация класса CChildView
//

#include "stdafx.h"
#include "Prim_FIT.h"
#include "ChildView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CChildView

CChildView::CChildView()
{
	Index = 0;
}

CChildView::~CChildView()
{
}


BEGIN_MESSAGE_MAP(CChildView, CWnd)
	ON_WM_PAINT()
	ON_COMMAND(ID_MYMENU_POINT1, &CChildView::OnMymenuPoint1)
	ON_COMMAND(ID_MYMENU_PRINT, &CChildView::OnMymenuPrint)
	ON_COMMAND(ID_MYMENU_ELLIPSE, &CChildView::OnMymenuEllipse)
	ON_COMMAND(ID_MYMENU_CIRCLE, &CChildView::OnMymenuCircle)
	ON_COMMAND(ID_MYMENU_PICTURE, &CChildView::OnMymenuPicture)
	ON_WM_LBUTTONDOWN()
END_MESSAGE_MAP()



// обработчики сообщений CChildView

BOOL CChildView::PreCreateWindow(CREATESTRUCT& cs) 
{
	if (!CWnd::PreCreateWindow(cs))
		return FALSE;

	cs.dwExStyle |= WS_EX_CLIENTEDGE;
	cs.style &= ~WS_BORDER;
	cs.lpszClass = AfxRegisterWndClass(CS_HREDRAW|CS_VREDRAW|CS_DBLCLKS, 
		::LoadCursor(NULL, IDC_ARROW), reinterpret_cast<HBRUSH>(COLOR_WINDOW+1), NULL);

	return TRUE;
}

void CChildView::OnPaint() 
{
	CPaintDC dc(this); // контекст устройства для рисования
	
	// TODO: Добавьте код обработки сообщений


	if (Index == 1) dc.TextOut(10, 10, _T("Вывод текста в окно Windows!"));
	if (Index == 2) dc.Ellipse(50,50,250,200);
	if (Index == 3) dc.Ellipse(100,100,200,200);

	
	// Не вызывайте CWnd::OnPaint() для сообщений рисования
}



void CChildView::OnMymenuPoint1()    // Обработчик пункта меню "Point1"
{
	// TODO: добавьте свой код обработчика команд

	 AfxMessageBox("Привет!!!  Я Point1");
	


}


void CChildView::OnMymenuPrint()
{
	// TODO: добавьте свой код обработчика команд
	Index = 1;
	this->Invalidate(1);
}


void CChildView::OnMymenuEllipse()
{
	// TODO: добавьте свой код обработчика команд
	Index = 2;
	this->Invalidate(1);
}


void CChildView::OnMymenuCircle()
{
	// TODO: добавьте свой код обработчика команд

	Index = 3;
	this->Invalidate();
}


void CChildView::OnMymenuPicture()
{
	// TODO: добавьте свой код обработчика команд
	Index = 0;
	CDC* pDC = GetDC();
	pDC->Ellipse(200, 200, 300, 500);
	
	
}


void CChildView::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: добавьте свой код обработчика сообщений или вызов стандартного

	CWnd::OnLButtonDown(nFlags, point);
	AfxMessageBox("Нажата левая клавиша мыши!");

}
