#pragma once

#include <spellcheck.h>
#include <stdio.h>
#include <vector>
#include <stdlib.h>

struct SpellError
{
public:
	ULONG32 Length;
	ULONG32 Start;
};

extern "C" __declspec(dllexport) void Init();
extern "C" __declspec(dllexport) ISpellCheckerFactory* CreateSpellCheckerFactory();
extern "C" __declspec(dllexport) ISpellChecker* CreateSpellChecker(ISpellCheckerFactory* factory, PCWSTR language);
extern "C" __declspec(dllexport) ULONG CheckSpelling(ISpellChecker* checker, PCWSTR str, SpellError* items);
extern "C" __declspec(dllexport) ULONG GetSuggestions(ISpellChecker* checker, PCWSTR str, wchar_t** items);


extern "C" __declspec(dllexport) void Release(ISpellCheckerFactory* factory, ISpellChecker* checker);