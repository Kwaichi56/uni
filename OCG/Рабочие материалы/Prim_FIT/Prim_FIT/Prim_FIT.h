
// Prim_FIT.h : главный файл заголовка для приложения Prim_FIT
//
#pragma once

#ifndef __AFXWIN_H__
	#error "включить stdafx.h до включения этого файла в PCH"
#endif

#include "resource.h"       // основные символы


// CPrim_FITApp:
// О реализации данного класса см. Prim_FIT.cpp
//

class CPrim_FITApp : public CWinApp
{
public:
	CPrim_FITApp();


// Переопределение
public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();

// Реализация

public:
	afx_msg void OnAppAbout();
	DECLARE_MESSAGE_MAP()
};

extern CPrim_FITApp theApp;
