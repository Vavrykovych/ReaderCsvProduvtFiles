using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProgrammingTask
{
    class ProductList
    {
        private List<Product> products;
        public List<Product> Products
        {
            get
            {
                return products;
            }
            set
            {
                products = value;
            }
        }

        public ProductList()
        {
            this.products = null;
        }
        public ProductList(List<Product> products)
        {
            this.products = products;
        }

        public void SaveAsCsv(string filepath)
        {
            StringBuilder data = new StringBuilder("");
            foreach(Product product in products)
            {
                data.Append(product.Name + "," + product.Produser + "," + product.Price.ToString() + "\n");
            }
            StreamWriter streamWriter = new StreamWriter(filepath);
            streamWriter.Write(data);
            streamWriter.Close();
        }

        public void SaveAsXml(string filepath)
        {
            XmlSerializer xmlser = new XmlSerializer(typeof(List<Product>));
            StreamWriter streamWriter = new StreamWriter(filepath);
            xmlser.Serialize(streamWriter, Products);
            streamWriter.Close();
        }
        


        public void LoadFromFile(string filepath)
        {
            StreamReader streamReader = new StreamReader(filepath);
            Products = new List<Product>();
            while(!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                var arr = line.Split(';', ',');
                Products.Add(new Product(arr[0],arr[1],Convert.ToDouble(arr[2])));;
            }
            streamReader.Close();
        }

    }
}