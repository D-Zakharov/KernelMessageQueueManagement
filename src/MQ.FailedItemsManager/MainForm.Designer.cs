namespace MQ.FailedItemsManager
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewFailedItems = new System.Windows.Forms.ListView();
            this.Id = new System.Windows.Forms.ColumnHeader();
            this.CreationDate = new System.Windows.Forms.ColumnHeader();
            this.ExceptionMessage = new System.Windows.Forms.ColumnHeader();
            this.SerializedItem = new System.Windows.Forms.ColumnHeader();
            this.btnSendBack = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewFailedItems
            // 
            this.listViewFailedItems.CheckBoxes = true;
            this.listViewFailedItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.CreationDate,
            this.ExceptionMessage,
            this.SerializedItem});
            this.listViewFailedItems.Location = new System.Drawing.Point(12, 12);
            this.listViewFailedItems.Name = "listViewFailedItems";
            this.listViewFailedItems.Size = new System.Drawing.Size(776, 376);
            this.listViewFailedItems.TabIndex = 1;
            this.listViewFailedItems.UseCompatibleStateImageBehavior = false;
            this.listViewFailedItems.View = System.Windows.Forms.View.Details;
            // 
            // Id
            // 
            this.Id.Text = "Id";
            // 
            // CreationDate
            // 
            this.CreationDate.Text = "Дата создания";
            this.CreationDate.Width = 120;
            // 
            // ExceptionMessage
            // 
            this.ExceptionMessage.Text = "Сообщение об ошибке";
            this.ExceptionMessage.Width = 220;
            // 
            // SerializedItem
            // 
            this.SerializedItem.Text = "Элемент очереди";
            this.SerializedItem.Width = 370;
            // 
            // btnSendBack
            // 
            this.btnSendBack.Location = new System.Drawing.Point(12, 415);
            this.btnSendBack.Name = "btnSendBack";
            this.btnSendBack.Size = new System.Drawing.Size(180, 23);
            this.btnSendBack.TabIndex = 2;
            this.btnSendBack.Text = "Отправить назад в очередь";
            this.btnSendBack.UseVisualStyleBackColor = true;
            this.btnSendBack.Click += new System.EventHandler(this.btnSendBack_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(713, 415);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "Удалить";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnSendBack);
            this.Controls.Add(this.listViewFailedItems);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Failed messages";
            this.ResumeLayout(false);

        }

        #endregion

        private ListView listViewFailedItems;
        private ColumnHeader Id;
        private ColumnHeader CreationDate;
        private ColumnHeader ExceptionMessage;
        private ColumnHeader SerializedItem;
        private Button btnSendBack;
        private Button btnRemove;
    }
}