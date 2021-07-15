﻿//
// This file is automatically generated by Facc
// https://github.com/fawdlstty/Fapp
//



#ifndef __FUNC_ARGS_AST_HPP__
#define __FUNC_ARGS_AST_HPP__



#include <iostream>
#include <memory>
#include <optional>
#include <string>
#include <string_view>

#include <fmt/core.h>

#include <Facc/IEnumerator.hpp>
#include <Facc/Parser/IAST.h>
#include <Facc/Parser/AstParser.hpp>



class SAST;
class TypeAST;
class IdAST;

class FuncArgsAST_4: IAST {
public:
	// [part of] func_args ::= s ',' s type s id

	std::shared_ptr<AstParser> Parser;

	IEnumerator<int> TryParse (int _pos) override {
		if (!Parser->TryReg ("FuncArgsAST_4", _pos))
			co_return;
		auto _4_0_enum = _try_parse_4_0 (_pos);
		while (_4_0_enum.MoveNext ()) {
			auto _4_1_enum = _try_parse_4_1 (_4_0_enum.Current);
			while (_4_1_enum.MoveNext ()) {
				auto _4_2_enum = _try_parse_4_2 (_4_1_enum.Current);
				while (_4_2_enum.MoveNext ()) {
					auto _4_3_enum = _try_parse_4_3 (_4_2_enum.Current);
					while (_4_3_enum.MoveNext ()) {
						auto _4_4_enum = _try_parse_4_4 (_4_3_enum.Current);
						while (_4_4_enum.MoveNext ()) {
							auto _4_5_enum = _try_parse_4_5 (_4_4_enum.Current);
							while (_4_5_enum.MoveNext ()) {
								co_yield _4_5_enum.Current;
							}
						}
					}
				}
			}
		}
		co_yield _pos;
		Parser->UnReg ("FuncArgsAST_4", _pos);
	}

	IEnumerator<int> _try_parse_4_0 (int _pos);
	IEnumerator<int> _try_parse_4_1 (int _pos);
	IEnumerator<int> _try_parse_4_2 (int _pos);
	IEnumerator<int> _try_parse_4_3 (int _pos);
	IEnumerator<int> _try_parse_4_4 (int _pos);
	IEnumerator<int> _try_parse_4_5 (int _pos);
	bool IsValid () override;
	void PrintTree (int _indent);
	int size ();

	std::shared_ptr<SAST> Value_4_0 = nullptr;
	std::string Value_4_1 = "";
	std::shared_ptr<SAST> Value_4_2 = nullptr;
	std::shared_ptr<TypeAST> Value_4_3 = nullptr;
	std::shared_ptr<SAST> Value_4_4 = nullptr;
	std::shared_ptr<IdAST> Value_4_5 = nullptr;
};



class FuncArgsAST: IAST {
public:
	// func_args ::= (s type s id (s ',' s type s id)*)?

	std::shared_ptr<AstParser> Parser;

	IEnumerator<int> TryParse (int _pos) override {
		if (!Parser->TryReg ("FuncArgsAST", _pos))
			co_return;
		auto _0_enum = _try_parse_0 (_pos);
		while (_0_enum.MoveNext ()) {
			auto _1_enum = _try_parse_1 (_0_enum.Current);
			while (_1_enum.MoveNext ()) {
				auto _2_enum = _try_parse_2 (_1_enum.Current);
				while (_2_enum.MoveNext ()) {
					auto _3_enum = _try_parse_3 (_2_enum.Current);
					while (_3_enum.MoveNext ()) {
						auto _4_enum = _try_parse_4 (_3_enum.Current);
						while (_4_enum.MoveNext ()) {
							co_yield _4_enum.Current;
						}
					}
				}
			}
		}
		co_yield _pos;
		Parser->UnReg ("FuncArgsAST", _pos);
	}

	IEnumerator<int> _try_parse_0 (int _pos);
	IEnumerator<int> _try_parse_1 (int _pos);
	IEnumerator<int> _try_parse_2 (int _pos);
	IEnumerator<int> _try_parse_3 (int _pos);
	IEnumerator<int> _try_parse_4 (int _pos);
	bool IsValid () override;
	void PrintTree (int _indent);
	int size ();

	std::shared_ptr<SAST> Value_0 = nullptr;
	std::shared_ptr<TypeAST> Value_1 = nullptr;
	std::shared_ptr<SAST> Value_2 = nullptr;
	std::shared_ptr<IdAST> Value_3 = nullptr;
	std::vector<std::shared_ptr<FuncArgsAST_4>> Value_4;
};



#include "SAST.hpp"
#include "TypeAST.hpp"
#include "IdAST.hpp"



inline bool FuncArgsAST_4::IsValid () {
	return true;
}

inline void FuncArgsAST_4::PrintTree (int _indent) {
	if (Value_4_0 && Value_4_0->IsValid ()) {
		Value_4_0->PrintTree (_indent + 1);
	}
	std::cout << std::string ((_indent + 1) * 4, ' ') << '[' << Value_4_1 << ']' << std::endl;
	if (Value_4_2 && Value_4_2->IsValid ()) {
		Value_4_2->PrintTree (_indent + 1);
	}
	if (Value_4_3 && Value_4_3->IsValid ()) {
		Value_4_3->PrintTree (_indent + 1);
	}
	if (Value_4_4 && Value_4_4->IsValid ()) {
		Value_4_4->PrintTree (_indent + 1);
	}
	if (Value_4_5 && Value_4_5->IsValid ()) {
		Value_4_5->PrintTree (_indent + 1);
	}
}

inline int FuncArgsAST_4::size () {
	int _len = 0;
	_len += Value_4_0->size ();
	_len += Value_4_1.size ();
	_len += Value_4_2->size ();
	_len += Value_4_3->size ();
	_len += Value_4_4->size ();
	_len += Value_4_5->size ();
	return _len;
}
inline IEnumerator<int> FuncArgsAST_4::_try_parse_4_0 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<SAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_4_0 = _o;
		co_yield _enum.Current;
		Value_4_0 = nullptr;
	}
}

inline IEnumerator<int> FuncArgsAST_4::_try_parse_4_1 (int _pos) {
	Parser->SetErrorPos (_pos);
	if (Parser->TryMatchString (_pos, ",")) {
		Value_4_1 = ",";
		co_yield _pos + Value_4_1.size ();
		Value_4_1 = "";
	}
}

inline IEnumerator<int> FuncArgsAST_4::_try_parse_4_2 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<SAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_4_2 = _o;
		co_yield _enum.Current;
		Value_4_2 = nullptr;
	}
}

inline IEnumerator<int> FuncArgsAST_4::_try_parse_4_3 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<TypeAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_4_3 = _o;
		co_yield _enum.Current;
		Value_4_3 = nullptr;
	}
}

inline IEnumerator<int> FuncArgsAST_4::_try_parse_4_4 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<SAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_4_4 = _o;
		co_yield _enum.Current;
		Value_4_4 = nullptr;
	}
}

inline IEnumerator<int> FuncArgsAST_4::_try_parse_4_5 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<IdAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_4_5 = _o;
		co_yield _enum.Current;
		Value_4_5 = nullptr;
	}
}

inline bool FuncArgsAST::IsValid () {
	return true;
}

inline void FuncArgsAST::PrintTree (int _indent) {
	if (Value_0 && Value_0->IsValid ()) {
		Value_0->PrintTree (_indent + 1);
	}
	if (Value_1 && Value_1->IsValid ()) {
		Value_1->PrintTree (_indent + 1);
	}
	if (Value_2 && Value_2->IsValid ()) {
		Value_2->PrintTree (_indent + 1);
	}
	if (Value_3 && Value_3->IsValid ()) {
		Value_3->PrintTree (_indent + 1);
	}
	for (size_t i = 0; i < Value_4.size (); ++i)
		Value_4 [i]->PrintTree (_indent + 1);
}

inline int FuncArgsAST::size () {
	int _len = 0;
	_len += Value_0->size ();
	_len += Value_1->size ();
	_len += Value_2->size ();
	_len += Value_3->size ();
	for (size_t i = 0; i < Value_4.size (); ++i)
		_len += Value_4 [i]->size ();
	return _len;
}
inline IEnumerator<int> FuncArgsAST::_try_parse_0 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<SAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_0 = _o;
		co_yield _enum.Current;
		Value_0 = nullptr;
	}
}

inline IEnumerator<int> FuncArgsAST::_try_parse_1 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<TypeAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_1 = _o;
		co_yield _enum.Current;
		Value_1 = nullptr;
	}
}

inline IEnumerator<int> FuncArgsAST::_try_parse_2 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<SAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_2 = _o;
		co_yield _enum.Current;
		Value_2 = nullptr;
	}
}

inline IEnumerator<int> FuncArgsAST::_try_parse_3 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<IdAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_3 = _o;
		co_yield _enum.Current;
		Value_3 = nullptr;
	}
}

inline IEnumerator<int> FuncArgsAST::_try_parse_4 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<FuncArgsAST_4> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		int _list_pos = Value_4.size ();
		Value_4.push_back (_o);
		co_yield _enum.Current;
		auto _enum1 = _try_parse_4 (_enum.Current);
		while (_enum1.MoveNext ())
			co_yield _enum1.Current;
		Value_4.erase (Value_4.begin () + _list_pos);
	}
	co_yield _pos;
}



#endif // __FUNC_ARGS_AST_HPP__