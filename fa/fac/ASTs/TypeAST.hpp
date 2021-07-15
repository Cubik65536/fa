﻿//
// This file is automatically generated by Facc
// https://github.com/fawdlstty/Fapp
//



#ifndef __TYPE_AST_HPP__
#define __TYPE_AST_HPP__



#include <iostream>
#include <memory>
#include <optional>
#include <string>
#include <string_view>

#include <fmt/core.h>

#include <Facc/IEnumerator.hpp>
#include <Facc/Parser/IAST.h>
#include <Facc/Parser/AstParser.hpp>



class IdsAST;

class TypeAST: IAST {
public:
	// type ::= ids

	std::shared_ptr<AstParser> Parser;

	IEnumerator<int> TryParse (int _pos) override {
		if (!Parser->TryReg ("TypeAST", _pos))
			co_return;
		auto _0_enum = _try_parse_0 (_pos);
		while (_0_enum.MoveNext ()) {
			co_yield _0_enum.Current;
		}
		Parser->UnReg ("TypeAST", _pos);
	}

	IEnumerator<int> _try_parse_0 (int _pos);
	bool IsValid () override;
	void PrintTree (int _indent);
	int size ();

	std::shared_ptr<IdsAST> Value_0 = nullptr;
};



#include "IdsAST.hpp"



inline bool TypeAST::IsValid () {
	return Value_0->IsValid ();
}

inline void TypeAST::PrintTree (int _indent) {
	if (Value_0 && Value_0->IsValid ()) {
		Value_0->PrintTree (_indent + 1);
	}
}

inline int TypeAST::size () {
	int _len = 0;
	_len += Value_0->size ();
	return _len;
}
inline IEnumerator<int> TypeAST::_try_parse_0 (int _pos) {
	Parser->SetErrorPos (_pos);
	auto _o = std::make_shared<IdsAST> ();
	_o->Parser = Parser;
	auto _enum = _o->TryParse (_pos);
	while (_enum.MoveNext ()) {
		Value_0 = _o;
		co_yield _enum.Current;
		Value_0 = nullptr;
	}
}



#endif // __TYPE_AST_HPP__