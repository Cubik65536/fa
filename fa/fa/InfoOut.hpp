#ifndef __INFO_OUT_HPP__
#define __INFO_OUT_HPP__



#include <iostream>
#include <string>

#include <antlr4-runtime/Token.h>



class InfoOut {
	static std::string _process_file (std::string _file) {
		size_t _p = _file.rfind ('/');
		size_t _q = _file.rfind ('\\');
		if (_p == std::string::npos) {
			if (_q == std::string::npos) {
				return _file;
			} else {
				return _file.substr (_q + 1);
			}
		} else {
			if (_q == std::string::npos) {
				return _file.substr (_p + 1);
			} else {
				return _file.substr ((_p > _q ? _p : _q) + 1);
			}
		}
	}

public:
	static void SetCurrentFile (std::string _file) {
		s_file = _process_file (_file);
	}
	static void Info (const char *_file, int _line, antlr4::Token *_t, std::string _data) {
		std::string _prefix = fmt::format ("[{}:{}] ", _process_file (_file), _line);
		std::string _content = _t ? fmt::format ("λ�� [{}:{} pos {}] ����Ϣ��", s_file, _t->getLine (), _t->getCharPositionInLine ()): "";
		std::cout << fmt::format ("{}{}{}", _prefix, _content, _data) << std::endl;
	}

	static void Warning (const char *_file, int _line, antlr4::Token *_t, std::string _data) {
		std::string _prefix = fmt::format ("[{}:{}] ", _process_file (_file), _line);
		std::string _content = _t ? fmt::format ("λ�� [{}:{} pos {}] �ľ��棺", s_file, _t->getLine (), _t->getCharPositionInLine ()): "";
		std::cout << fmt::format ("{}{}{}", _prefix, _content, _data) << std::endl;
	}

	static void Error (const char *_file, int _line, antlr4::Token *_t, std::string _data) {
		std::string _prefix = fmt::format ("[{}:{}] ", _process_file (_file), _line);
		std::string _content = _t ? fmt::format ("λ�� [{}:{} pos {}] �Ĵ���", s_file, _t->getLine (), _t->getCharPositionInLine ()) : "";
		std::cout << fmt::format ("{}{}{}", _prefix, _content, _data) << std::endl;
	}

	static std::string s_file;
};

__declspec (selectany) std::string InfoOut::s_file = "";

#define LOG_INFO(t,data)		InfoOut::Info(__FILE__,__LINE__,t,data)
#define LOG_WARNING(t,data)		InfoOut::Warning(__FILE__,__LINE__,t,data)
#define LOG_ERROR(t,data)		InfoOut::Error(__FILE__,__LINE__,t,data)
#define LOG_TODO(t)				InfoOut::Error(__FILE__,__LINE__,t,"�˹�����δ���")



#endif //__INFO_OUT_HPP__