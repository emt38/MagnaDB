using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
{
    public enum LandVehicleType : byte
    {
        Sedan,
        Coupe,
        Crossover,
        SUV,
        Truck
    }

    public class SampleModel : TableModel<SampleModel>
    {
        protected override string ConnectionString
        {
            get
            {
                /*
                    Execute the following Query to your SQL Database for testing purposes

                CREATE TABLE [dbo].[SampleModels](
	                [ModelId] [int] IDENTITY(1,1) NOT NULL,
	                [Brand] [varchar](50) NOT NULL,
	                [Horsepower] [int] NULL,
	                [ModelName] [varchar](50) NOT NULL,
	                [ModelYear] [int] NULL,
	                [Type] [tinyint] NULL,
                 CONSTRAINT [PK_SampleModels] PRIMARY KEY CLUSTERED 
                (
	                [ModelId] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                GO

                */

                // Specify your Database's Connection String
                return @"";
            }
        }

        protected override MagnaKey Key
        {
            get
            {
                // You need to return a New MagnaKey object with all fields representing the primary key
                // For this purposes you can use the MakeKey Extension which is a short hand to do so
                return this.MakeKey(c => c.ModelId);
            }
        }

        protected override string TableName
        {
            get
            {
                // Specify the TableName for your TableModel
                return "SampleModels";
            }
        }


        public SampleModel()
        {
            // It's convenient to assign the EventHandlers inside the constructor
            // It will keep your model definitions centralized
            // You can cast the sender object to the class you're defining
            InsertSucceeded += SampleModel_InsertSucceeded;
        }

        private void SampleModel_InsertSucceeded(object sender, MagnaEventArgs e)
        {
            SampleModel temp = sender as SampleModel;
            Console.WriteLine("This Model is Alive, and it's ID: {0}", temp.ModelId);
        }

        // If you're using an Identity field in your DB you need to specify it in your model
        // That way it will not be inserted and will be updated if the model object is inserted
        [Identity]
        public int ModelId { get; set; }

        // All Properties decorated with the DuplicationColumn will be evaluated as a possible
        // Existing Key within the database using the IsDuplicatedMethod
        // You can specify an index parameter in the Constructor to create different
        // key combinations. DEFAULT 0
        [DuplicationColumn]
        public string Brand { get; set; }

        public DateTime? MarketReleaseDate { get; set; }
        public int Horsepower { get; set; }

        [DuplicationColumn]
        public string ModelName { get; set; }

        [DuplicationColumn]
        public int ModelYear { get; set; }

        public LandVehicleType Type { get; set; }
    }
}
