using System;

namespace WMIT.Framework.Test.VO.Parametros
{
    public class Usuario : Framework.VO.Parametros
    {
        public int Key { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public int Perfil { get; set; }
        public int TipoPessoa { get; set; }
        public int Sexo { get; set; }

        private Framework.VO.Between<DateTime> _Nascimento;
        public Framework.VO.Between<DateTime> Nascimento
        {
            get { return _Nascimento ?? (_Nascimento = new Framework.VO.Between<DateTime>()); }
            set { _Nascimento = value; }
        }
        
        private Framework.VO.Between<DateTime> _Cadastro;
        public override Framework.VO.Between<DateTime> Cadastro
        {
            get { return _Cadastro ?? (_Cadastro = new Framework.VO.Between<DateTime>()); }
            set { _Cadastro = value; }
        }

        public static class FieldsBitwise
        {
            public const int Codigo = 1 << 0;
            public const int Key = 1 << 1;
            public const int Email = 1 << 2;
            public const int Nome = 1 << 3;
            public const int Nascimento = 1 << 4;
            public const int Cadastro = 1 << 5;
        }

        public static class EntitiesBitwise
        {
            public const int TipoPessoa = 1 << 0;
            public const int Perfil = 1 << 1;
            public const int Sexo = 1 << 2;
            public const int Endereco = 1 << 3;
        }
    }
}
