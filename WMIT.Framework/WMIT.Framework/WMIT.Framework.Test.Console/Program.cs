using System;
using System.Collections.Generic;
using System.Configuration;

namespace WMIT.Framework.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch lUnitWatch = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch lTotalWatch = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch lBLWatch = new System.Diagnostics.Stopwatch();
            var counter = 0;
            IList<VO.Usuario> lUsuarios = new List<VO.Usuario>();
            var crypto = Utilitarios.Crypto.Encrypt("admin", ConfigurationManager.AppSettings["CryptoKey"]);

            //lBLWatch.Start();
            lTotalWatch.Start();
            //using (BLL.Usuario objBLL = new BLL.Usuario(new VO.Usuario() { Codigo = 1 }))
            //{
            //System.Console.WriteLine(lBLWatch.ElapsedMilliseconds);

            //objBLL.BeginTransaction();

            //System.Console.WriteLine(lBLWatch.ElapsedMilliseconds);
            //lBLWatch.Stop();


            //lUnitWatch.Start();
            var i = 0;
            do
            {
                counter = 0;
                lTotalWatch.Restart();

                var lOrdinal = new VO.Ordinal.Usuario();
                lOrdinal.Fields.Add(lOrdinal.Field.AllNonAudit);
                lOrdinal.UsuarioPerfil.Fields.Add(lOrdinal.UsuarioPerfil.Field.AllNonAudit);

                Model.Repository.IRepositoryUsuario lRepository = new Repository.RepositorioUsuario();

                do
                {
                    var lCollection = lRepository.Get(null, lOrdinal);
                } while (++counter < 100);

                lTotalWatch.Stop();
                System.Console.WriteLine(String.Concat("Total: ", lTotalWatch.ElapsedMilliseconds));

                counter = 0;
                lTotalWatch.Restart();
                do
                {
                    using (BLL.Usuario objBLL = new BLL.Usuario())
                    {

                        objBLL.Ordinal.Fields.Add(objBLL.Ordinal.Field.AllNonAudit);
                        objBLL.Ordinal.UsuarioPerfil.Fields.Add(objBLL.Ordinal.UsuarioPerfil.Field.AllNonAudit);

                        objBLL.Get();


                        //        //objBLL.Salvar(new VO.Usuario()
                        //        //{
                        //        //    Nome = string.Concat("Teste de performace", counter),
                        //        //    Aceite = true,
                        //        //    Senha = crypto,
                        //        //    Nascimento = DateTime.Today,
                        //        //    UsuarioPerfil = new VO.UsuarioPerfil() { Codigo = 1 },
                        //        //    UsuarioSexo = new VO.UsuarioSexo() { Codigo = 1 },
                        //        //    UsuarioTipoPessoa = new VO.UsuarioTipoPessoa() { Codigo = 1 }
                        //        //});

                        //        //    lUsuarios = objBLL.Collection;

                        //        //System.Console.WriteLine(lUnitWatch.ElapsedMilliseconds);
                    }
                    //    //System.Console.WriteLine(lUnitWatch.Elapsed.Milliseconds);
                    //    //lUnitWatch.Restart();
                } while (++counter < 100);
                //objBLL.CommitTransaction();

                lTotalWatch.Stop();
                System.Console.WriteLine(String.Concat("Total: ", lTotalWatch.ElapsedMilliseconds));

                //counter = 0;
                //lTotalWatch.Restart();

                //var lOrdinal = new VO.Ordinal.Usuario();
                //lOrdinal.Fields.Add(lOrdinal.Field.AllNonAudit);
                //lOrdinal.UsuarioPerfil.Fields.Add(lOrdinal.UsuarioPerfil.Field.AllNonAudit);

                //Model.Repository.IRepositoryUsuario lRepository = new Repository.RepositorioUsuario();

                //do
                //{
                //    var lCollection = lRepository.Get(null, lOrdinal);
                //} while (++counter < 100);

                //lTotalWatch.Stop();
                //System.Console.WriteLine(String.Concat("Total: ", lTotalWatch.ElapsedMilliseconds));

                System.Console.WriteLine("-------------------------");
            } while (++i < 5);

            //lUnitWatch.Stop();
            System.Console.ReadKey();
        }
    }
}
