using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Median_cut
{   public class mpix {
        //new class to use insted of Color
        public int R; //red
        public int G; //green
        public int B; //blue
        public int x; //pos on x
        public int y; //pos on y

        public mpix(Color c,int x,int y) {

            this.R = c.R;
            this.G = c.G;
            this.B = c.B;
            this.x = x;
            this.y = y;
        }
    }
    
    public class cube
    {
        //cube structure
        public List<mpix> my_cube = new List<mpix>(); //list of color in the cube
        public Tuple<int, int> axe_max; //axe to divade on and max range of that axe

        public cube (List<mpix> my_cube)
        {
            this.my_cube = my_cube;
        }

        public Tuple<int, int> c_max_min_axe(List<mpix> my_list)
        {
            // item1 is the axe number ie 0 for red ,1 for green ,2 for blue
            // item2  is the  max of max-min of every axe
            int maxr = 0;
            int maxg = 0;
            int maxb = 0;
            int minr = 255;
            int ming = 255;
            int minb = 255;
            
            //getting the max of each color
            foreach (mpix c in my_list)
            {
                if (maxr < c.R) { maxr = c.R; }
                if (maxg < c.G) { maxg = c.G; }
                if (maxb < c.B) { maxb = c.B; }

                if (minr > c.R) { minr = c.R; }
                if (ming > c.G) { ming = c.G; }
                if (minb > c.B) { minb = c.B; }
            }

            int[] max_min = new int[3];
            max_min[0] = maxr - minr;
            max_min[1] = maxg - ming;
            max_min[2] = maxb - minb;

            //max range
            List<int> levels = max_min.AsEnumerable().ToList();
            int max_color = levels.Max();

            if (max_min[0] == max_color) { return Tuple.Create(0, max_color); }
            if (max_min[1] == max_color) { return Tuple.Create(1, max_color); }
            if (max_min[2] == max_color) { return Tuple.Create(2, max_color); }
            else
            {
                return Tuple.Create(-1, -1);
            }
        }

        public void order_my_list(List<mpix> color, int axe)
        {   
            //ordaring the list based on a giving axe
            List<mpix> orderl = new List<mpix>();

            if (axe == 0) { orderl.AddRange(color.OrderBy(o => o.R).ToList()); }
            if (axe == 1) { orderl.AddRange(color.OrderBy(o => o.G).ToList()); }
            if (axe == 2) { orderl.AddRange(color.OrderBy(o => o.B).ToList()); }

            this.my_cube.Clear();
            this.my_cube.AddRange(orderl);


        }


        public Tuple<List<mpix>, List<mpix>> divade_list(List<mpix> main_list)
        {
            //cutting a list in the median pos

            List<mpix> l1 = new List<mpix>();
            List<mpix> l2 = new List<mpix>();


            int end1 = (main_list.Count) / 2;
            int start2 = end1;
            int end2 = main_list.Count - start2;

            l1.AddRange(main_list.GetRange(0, end1));
            l2.AddRange(main_list.GetRange(start2, end2));
            return Tuple.Create(l1, l2);

        }

        public List<mpix> mean_of_list(List<mpix> main_list)
        {   
            //getting the mean a list 

            int meanr = 0;
            int meang = 0;
            int meanb = 0;

            foreach (mpix c in main_list)
            {
                meanr = meanr + c.R;
                meang = meang + c.G;
                meanb = meanb + c.B;
            }

            foreach (mpix c in main_list)
            {
                c.R = meanr / main_list.Count;
                c.G = meang / main_list.Count;
                c.B = meanb / main_list.Count;
            }

            return main_list;
        }

        public Tuple<List<mpix>, List<mpix>> run_mc()
        {
            //runing median cut on the current cube

            //getting axe
            this.axe_max = c_max_min_axe(this.my_cube);
            //Console.WriteLine("here1");
            //orrdaring list
            this.order_my_list(this.my_cube,this.axe_max.Item1);
            //Console.WriteLine("here2");

            List<mpix> l1 = new List<mpix>();
            List<mpix> l2 = new List<mpix>();

            l1.AddRange(this.divade_list(this.my_cube).Item1);
            l2.AddRange(this.divade_list(this.my_cube).Item2);

            
            //Console.WriteLine("here3");

            return Tuple.Create(l1, l2);

        }
    }


    public class mc
    {
        public Bitmap img; // image 
        public int nb_color; //number of color to be generated
        public List<cube> list_of_cube = new List<cube>();   //list of list of colors
        public int nb_of_distanc_val; //distance value


        public mc(Bitmap img){this.img = img;}

       

        public void create_main_list()
        {   //initing the main list of color and the 1st cube
            List<mpix> img_data = new List<mpix>();
            for (int x = 0; x < this.img.Width; x++)
            {
                for (int y = 0; y < this.img.Height; y++)
                {

                    img_data.Add(new mpix(img.GetPixel(x, y), x, y));

                }
            }

            this.list_of_cube.Add(new cube(img_data));

            
            this.nb_of_distanc_val = (from x in img_data select x).Distinct().Count();            

        }

        public Bitmap median_cut()
        {   // runing median-cut algorithm on ouer image to get the new one 
            Bitmap median_cut_img = new Bitmap(this.img.Width, this.img.Height);
            
            for (int i = 0; i < nb_color; i++)
            {
                Console.WriteLine("len:" + list_of_cube.Count);
                if (list_of_cube.Count >= nb_color) { break; }
                
                List<cube> new_list_of_cube = new List<cube>();

                foreach (cube c in this.list_of_cube)
                {
                    Tuple<List<mpix>, List<mpix>> sub_lit = c.run_mc();
                    new_list_of_cube.Add(new cube(sub_lit.Item1));
                    new_list_of_cube.Add(new cube(sub_lit.Item2));
                }

                list_of_cube.Clear();
                list_of_cube.AddRange(new_list_of_cube);
            }



            
            foreach (cube c in list_of_cube)
            {
                c.my_cube = c.mean_of_list(c.my_cube);
                foreach(mpix pix in c.my_cube) 
                {
                    median_cut_img.SetPixel(pix.x, pix.y, Color.FromArgb(pix.R, pix.G, pix.B));
                }
            }

            this.list_of_cube.Clear();
            

            return median_cut_img;
        }



    }




}
