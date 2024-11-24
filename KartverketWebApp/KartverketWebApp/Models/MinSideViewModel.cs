using KartverketWebApp.Data;
using System.Collections.Generic;

namespace KartverketWebApp.Models
{
   public class MinSideViewModel
   {
       public int BrukerId { get; set; }
       public string Email { get; set; }
       public string BrukerType { get; set; }
       public string Fornavn { get; set; }
       public string Etternavn { get; set; }
       public ICollection<Rapport> Rapporter { get; set; }
       public Bruker Bruker { get; set; }
       public Person Person { get; set; }
   }

   public class DeleteUserRequest
   {
       public string UserEmail { get; set; }
   }
}