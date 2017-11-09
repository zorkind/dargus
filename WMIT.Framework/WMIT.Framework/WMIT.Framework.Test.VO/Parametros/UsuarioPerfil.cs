
namespace WMIT.Framework.Test.VO.Parametros
{
    public class UsuarioPerfil : Framework.VO.Parametros
    {
        public string Descricao { get; set; }

        public static class FieldsBitwise
        {
            public const int Codigo = 1 << 0;
            public const int Descricao = 1 << 1;
        }
    }
}
