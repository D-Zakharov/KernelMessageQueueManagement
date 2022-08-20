using System.Configuration;
using System.Runtime;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using MQ.Domain.Database;
using MQ.Domain.Database.Models;
using MQ.Domain.Queue.Services;
using MQ.Domain.Rabbit;
using RabbitMQ.Client;

namespace MQ.FailedItemsManager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadFailedItems();
        }

        private void LoadFailedItems()
        {
            using (var dbCtx = GetDbContext())
            {
                LoadFailedItems(dbCtx);
            }
        }
        private void LoadFailedItems(KernelDbContext dbCtx)
        {
            listViewFailedItems.Items.Clear();
            foreach (var failedItem in dbCtx.FailedItems.Where(i => i.ItemType == ItemTypes.DocImport))
            {
                ListViewItem item = new ListViewItem(failedItem.Id.ToString());
                item.SubItems.Add(failedItem.CreationDate.ToString("dd.MM.yyyy HH:mm"));
                item.SubItems.Add(failedItem.ExceptionMessage);
                item.SubItems.Add(failedItem.SerializedItem);
                listViewFailedItems.Items.Add(item);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            using (var dbCtx = GetDbContext())
            {
                foreach (ListViewItem item in listViewFailedItems.CheckedItems)
                {
                    var dbItem = dbCtx.FailedItems.Find(int.Parse(item.SubItems[0].Text));
                    dbCtx.FailedItems.Remove(dbItem!);
                }
                dbCtx.SaveChanges();
                LoadFailedItems(dbCtx);
            }
        }

        private static KernelDbContext GetDbContext()
        {
            DbContextOptionsBuilder<KernelDbContext> builder = new();
            builder.UseSqlServer(Program.Configuration.GetValue<string>("ConnectionString"));
            return new KernelDbContext(builder.Options);
        }

        private void btnSendBack_Click(object sender, EventArgs e)
        {
            var rabbitConfig = Program.Configuration!.GetSection(nameof(RabbitConfiguration)).Get<RabbitConfiguration>();
            rabbitConfig.CheckBasicProperties();

            IConnectionFactory RabbitConnectionFactory = rabbitConfig.CreateConnectionFactory();

            using (var rabbitConnection = RabbitConnectionFactory.CreateConnection())
            using (var model = rabbitConnection.CreateModel())
            using (var dbCtx = GetDbContext())
            using (var itemsService = new FailedItemsDbService(dbCtx))
            {
                model.DeclareDocImportQueue(rabbitConfig);

                foreach (ListViewItem item in listViewFailedItems.CheckedItems)
                {
                    itemsService.ReturnFailedItemToQueue(int.Parse(item.SubItems[0].Text), model, rabbitConfig.DocsQueueName!);
                }

                LoadFailedItems(dbCtx);
            }
        }
    }
}