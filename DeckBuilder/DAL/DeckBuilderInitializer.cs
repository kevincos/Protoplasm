using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DeckBuilder.Models;

namespace DeckBuilder.DAL
{
    
    //public class DeckBuilderInitializer : DeckBuilder.App_Start.DontDropDbJustCreateTablesIfModelChanged<DeckBuilderContext>
    public class DeckBuilderInitializer : DropCreateDatabaseIfModelChanges<DeckBuilderContext>    
    {
        protected override void Seed(DeckBuilderContext context)
        {
        }
    }
}