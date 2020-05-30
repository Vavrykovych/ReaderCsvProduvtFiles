using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammingTask
{
    public class Product
    {
        private string name;
        public string Name
        {
            get
            {
                return name;        
            }
            set
            {
                name = value;
            }
        }

        private string produser;
        public string Produser
        {
            get
            {
                return produser;
            }
            set
            {
                produser = value;
            }  
        }

        private double price;
        public double Price
        {
            get
            {
                return price;
            }
            set
            {
                if (value >= 0)
                {
                    price = value;
                }
            }
        }

        public Product()
        {
            this.Name = "";
            this.Produser = "";
            this.Price = 0;
        }
        public Product(string name,string produser, double price)
        {
            this.Name = name;
            this.Produser = produser;
            this.Price = price;
        }


    }
}
