﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fac.ASTs.Types {
	class AstType_Template: IAstType {
		public AstClass Class { get; set; }
		public List<IAstType> TemplateTypes { get; set; }



		public override (string, string) GenerateCSharp (int _indent) {
			throw new NotImplementedException ();
		}
	}
}