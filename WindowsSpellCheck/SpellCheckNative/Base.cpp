#include "Base.h"





HRESULT CreateSpellCheckerFactory(ISpellCheckerFactory** spellCheckerFactory)
{
	HRESULT hr = CoCreateInstance(__uuidof(SpellCheckerFactory), nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(spellCheckerFactory));
	if (FAILED(hr))
		*spellCheckerFactory = nullptr;
	return hr;
}

HRESULT CreateSpellChecker(ISpellCheckerFactory* spellCheckFactory, PCWSTR languageTag, ISpellChecker** spellChecker)
{
	BOOL isSupported = FALSE;
	HRESULT hr = spellCheckFactory->IsSupported(languageTag, &isSupported);
	if (SUCCEEDED(hr))
	{
		if (FALSE == isSupported)
			throw std::invalid_argument("This language is not supported on this pc");
		else
			hr = spellCheckFactory->CreateSpellChecker(languageTag, spellChecker);
	}
	return hr;
}

extern "C" __declspec(dllexport) void Init() {
	CoInitializeEx(nullptr, COINIT_MULTITHREADED);
}

extern "C" __declspec(dllexport) ISpellCheckerFactory* CreateSpellCheckerFactory()
{
	ISpellCheckerFactory* spellCheckerFactory = nullptr;
	HRESULT hr = CreateSpellCheckerFactory(&spellCheckerFactory);

	if (SUCCEEDED(hr))
		return spellCheckerFactory;
	return nullptr;
}


extern "C" __declspec(dllexport) ISpellChecker* CreateSpellChecker(ISpellCheckerFactory* factory, PCWSTR language)
{
	ISpellChecker* checker = NULL;
	HRESULT result = CreateSpellChecker(factory, language, &checker);
	if (SUCCEEDED(result))
		return checker;
	return nullptr;
}


extern "C" __declspec(dllexport) ULONG CheckSpelling(ISpellChecker* checker, PCWSTR str, SpellError* items)
{
	IEnumSpellingError* errors = nullptr;
	checker->Check(str, &errors);

	ISpellingError* err = nullptr;
	ULONG cval = 0;
	while (SUCCEEDED(errors->Next(&err)) && cval < 50)
	{
		if (err == nullptr)
			break;

		SpellError* error = new SpellError();

		ULONG length = 0;
		ULONG start = 0;
		LPWSTR str = NULL;

		HRESULT res = err->get_Length(&length);
		res = err->get_StartIndex(&start);
		res = err->get_Replacement(&str);
		error->Length = length;
		//error->Replacement = str;
		error->Start = start;

		items[cval] = *error;
		cval++;
	}
	return cval;
}

extern "C" __declspec(dllexport) ULONG GetSuggestions(ISpellChecker* checker, PCWSTR str, wchar_t** items)
{
	IEnumString* strs;
	HRESULT res = checker->Suggest(str, &strs);

	int wLength = wcslen(str) + 4;

	ULONG count = 0;
	if (SUCCEEDED(res))
	{
		wchar_t * suggestion;

		int size = sizeof(wchar_t);
	
		while (SUCCEEDED(strs->Next(1, &items[count * wLength], nullptr)) && count < 20)
		{
			if (items[count * wLength] == nullptr)
				break;
			count++;
			if (count < 20)
				items[count * wLength] = nullptr;
		}

		return count;
	}
	return 0;
}


extern "C" __declspec(dllexport) void Release(ISpellCheckerFactory* factory, ISpellChecker* checker)
{
	if (checker != nullptr)
	{
		checker->Release();
		delete checker;
	}
	if (factory != nullptr)
	{
		factory->Release();
		delete factory;
	}
}