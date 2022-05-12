using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Eyes
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {

        private DataBase.Agent editAgent;
        //private List<DataBase.ProductSale> newProduuctSale;

        public EditWindow(DataBase.Agent editObject)
        {
            InitializeComponent();
            editAgent = editObject;
            ComboBoxAgentType.ItemsSource = DataBase.Entities.GetContext().AgentType.ToList();
            ComboBoxProduct.ItemsSource = DataBase.Entities.GetContext().Product.ToList();

            foreach (DataBase.ProductSale addProductSale in editObject.ProductSale)
            {
                DataGridAgentList.Items.Add(addProductSale);
            }

            TextBoxTitleAgent.Text = editAgent.Title;
            TextBoxAdress.Text = editAgent.Address;
            ComboBoxAgentType.SelectedValue = editAgent.ID;
            TextBoxINN.Text = editAgent.INN;
            TextBoxKPP.Text = editAgent.KPP;
            TextBoxDirectorName.Text = editAgent.DirectorName;
            TextBoxPhone.Text = editAgent.Phone;
            TextBoxEmail.Text = editAgent.Email;
            TextBoxPriority.Text = editAgent.Priority.ToString();
            TextBoxFotoName.Text = editAgent.Logo;

        }


        private void selectedFoto(object sender, RoutedEventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

            if (op.ShowDialog() == true)
            {
                FotoMaterial.Source = new BitmapImage(new Uri(op.FileName));
                TextBoxFotoName.Text = op.FileName;
            }
        }

        private void clickAddNewAgent(object sender, RoutedEventArgs e)
        {
            double checkINN;
            string MessageError = "";
            if (!double.TryParse(TextBoxINN.Text, out checkINN) && checkINN < 999999999 && checkINN > 10000000000)
            {
                MessageError = "ИНН должен содержать 10 цифр!";
            }
            if (TextBoxTitleAgent.Text.Length < 2)
            {
                MessageError += "\nЗаполните наименование!";
            }
            if (TextBoxAdress.Text.Length < 2)
            {
                MessageError += "\nЗаполните адресс!";
            }
            if (ComboBoxAgentType.SelectedIndex == -1)
            {
                MessageError += "\nВыберите тип агента!";
            }
            if (TextBoxKPP.Text.Length < 9 || TextBoxKPP.Text.Length > 10)
            {
                MessageError += "\nКПП должен состоять из 9 символов!";
            }
            if (TextBoxDirectorName.Text.Length < 2)
            {
                MessageError += "\nЗаполните ФИО директора!";
            }
            if (TextBoxPhone.Text.Length < 2)
            {
                MessageError += "\nЗаполните телефон!";
            }
            if (TextBoxEmail.Text.Length < 2)
            {
                MessageError += "\nЗаполните Email!";
            }
            int Prioritet;
            if (!int.TryParse(TextBoxPriority.Text, out Prioritet) && Prioritet <= 0)
            {
                MessageError += "\nПриоритет должен быть больше 0!";
            }
            if (TextBoxFotoName.Text == "Выберите фотографию")
            {
                TextBoxFotoName.Text = "/Image/picture.png";
            }
            if (MessageError.Length > 1)
            {
                MessageBox.Show(MessageError);
            }
            else
            {
                //try
                //{
                    editAgent.Title = TextBoxTitleAgent.Text;
                    editAgent.AgentTypeID = int.Parse(ComboBoxAgentType.SelectedValue.ToString());
                    editAgent.Address = TextBoxAdress.Text;
                    editAgent.INN = TextBoxINN.Text;
                    editAgent.KPP = TextBoxKPP.Text;
                    editAgent.DirectorName = TextBoxDirectorName.Text;
                    editAgent.Phone = TextBoxPhone.Text;
                    editAgent.Email = TextBoxEmail.Text;
                    editAgent.Logo = TextBoxFotoName.Text;
                    editAgent.Priority = int.Parse(TextBoxPriority.Text);
                    DataBase.Entities.GetContext().SaveChanges();
                    DataBase.Entities.GetContext().ProductSale.RemoveRange(editAgent.ProductSale);
                    DataBase.Entities.GetContext().SaveChanges();

                    if (DataGridAgentList.Items.Count > 0)
                    {
                        for (int i = 0; i < DataGridAgentList.Items.Count; i++)
                        {
                            editAgent.ProductSale.Add(DataGridAgentList.Items[i] as DataBase.ProductSale);
                            DataBase.Entities.GetContext().SaveChanges();
                        }
                    }

                    

                    MessageBox.Show("Запись была успешно сохраненна в БД");
                    this.Close();

                    //Обновляем в главном меню Лист, в котором содержиться копия материалов из БД
                    (this.Owner as EyesMainWindow).updateAllAgentlList();

                //}
                //catch
                //{
                //    MessageBox.Show("Произошла непредвиденная ошибка");
                //}
            }
        }

        //Добавление истории продаж
        private void clickButtonAddAgent(object sender, RoutedEventArgs e)
        {
            int productCount;
            if (ComboBoxProduct.SelectedIndex != -1 && int.TryParse(ProductCount.Text, out productCount) && productCount > 0)
            {
                DataBase.ProductSale newProductSale = new DataBase.ProductSale();
                newProductSale.ProductID = int.Parse(ComboBoxProduct.SelectedValue.ToString());
                newProductSale.SaleDate = DateTime.Now;
                newProductSale.ProductCount = productCount;

                DataGridAgentList.Items.Add(newProductSale);
                
            }
            else
                MessageBox.Show("Необходимо выбрать возможного поставщика из выпадающего списка, а также ввести колличество проданного продукта");
        }

        //Удалении из истории продаж
        private void DeletedAt(object sender, RoutedEventArgs e)
        {
            DataGridAgentList.Items.Remove(DataGridAgentList.SelectedItem as DataBase.ProductSale);
        }

    }
}
