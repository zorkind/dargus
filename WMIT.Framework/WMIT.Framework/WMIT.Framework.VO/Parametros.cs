using System;
using System.Collections.Generic;

namespace WMIT.Framework.VO
{
    public abstract class Parametros : IParametros
    {
        /// <summary>
        /// Por default, seta a propriedade Ativo = true
        /// </summary>
        public Parametros()
        {
            Cadastro = new Between<DateTime>();
            Atualizacao = new Between<DateTime>();
            Ativo = true; 
        }
        /// <summary>
        /// Por default, seta a propriedade Ativo = true
        /// </summary>
        /// <param name="ConnectionString"></param>
        public Parametros(string ConnectionString)
        {
            Cadastro = new Between<DateTime>();
            Atualizacao = new Between<DateTime>();
            Ativo = true;
        }

        public int UsuarioCadastro { get; set; }
        public int UsuarioAtualizacao { get; set; }
        public virtual Between<DateTime> Cadastro { get; set; }
        public virtual Between<DateTime> Atualizacao { get; set; }

        public bool EnableGroup { get; set; }
        public bool? Ativo { get; set; }
        public int Codigo { get; set; }
        public int Top { get; set; }

        private List<int> _Codigos;
        public List<int> Codigos
        {
            get
            {
                if (_Codigos == null)
                    _Codigos = new List<int>();

                return _Codigos;
            }
            set
            {
                _Codigos = value;
            }
        }

        private string[] _OrderBy;
        public string[] OrderBy
        {
            get
            {
                if (_OrderBy == null)
                    _OrderBy = new string[] {};

                return _OrderBy;
            }
            set
            {
                _OrderBy = value;
            }
        }

        private Paginate _Paginate;
        public Paginate Paginate
        {
            get
            {
                if (_Paginate == null)
                    _Paginate = new Paginate();

                return _Paginate;
            }
            set
            {
                _Paginate = value;
            }
        }

        public virtual void EvaluateParameters()
        { }
    }
}
