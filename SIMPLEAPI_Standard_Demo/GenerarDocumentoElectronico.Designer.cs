namespace SIMPLEAPI_Demo
{
    partial class GenerarDocumentoElectronico
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.numericFolio = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.botonGenerar = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkUnidad = new System.Windows.Forms.CheckBox();
            this.numericPrecio = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.botonAgregarLinea = new System.Windows.Forms.Button();
            this.checkAfecto = new System.Windows.Forms.CheckBox();
            this.numericCantidad = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.textNombre = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gridResultados = new System.Windows.Forms.DataGridView();
            this.gridNombreProducto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridCantidadProducto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridPrecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridAfecto = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.umedida = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridEliminar = new System.Windows.Forms.DataGridViewImageColumn();
            this.numericCasoPrueba = new System.Windows.Forms.NumericUpDown();
            this.labelCasoPrueba = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textGiroEmisor = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textComunaEmisor = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textDireccionEmisor = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textRazonSocialEmisor = new System.Windows.Forms.TextBox();
            this.textRUTEmisor = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textGiroReceptor = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textComunaReceptor = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textDireccionReceptor = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textRazonSocialReceptor = new System.Windows.Forms.TextBox();
            this.textRUTReceptor = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboTipo = new System.Windows.Forms.ComboBox();
            this.checkSetPruebas = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericFolio)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPrecio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCantidad)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridResultados)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCasoPrueba)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericFolio
            // 
            this.numericFolio.Location = new System.Drawing.Point(325, 20);
            this.numericFolio.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericFolio.Name = "numericFolio";
            this.numericFolio.Size = new System.Drawing.Size(65, 20);
            this.numericFolio.TabIndex = 13;
            this.numericFolio.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(287, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Folio:";
            // 
            // botonGenerar
            // 
            this.botonGenerar.Image = global::SIMPLEAPI_Demo.Properties.Resources.Guardar_32;
            this.botonGenerar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.botonGenerar.Location = new System.Drawing.Point(470, 494);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(149, 38);
            this.botonGenerar.TabIndex = 11;
            this.botonGenerar.Text = "Generar Documento";
            this.botonGenerar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.botonGenerar.UseVisualStyleBackColor = true;
            this.botonGenerar.Click += new System.EventHandler(this.botonGenerar_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkUnidad);
            this.groupBox2.Controls.Add(this.numericPrecio);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.botonAgregarLinea);
            this.groupBox2.Controls.Add(this.checkAfecto);
            this.groupBox2.Controls.Add(this.numericCantidad);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textNombre);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 253);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(607, 59);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Nuevo Producto";
            // 
            // checkUnidad
            // 
            this.checkUnidad.AutoSize = true;
            this.checkUnidad.Location = new System.Drawing.Point(492, 27);
            this.checkUnidad.Name = "checkUnidad";
            this.checkUnidad.Size = new System.Drawing.Size(60, 17);
            this.checkUnidad.TabIndex = 16;
            this.checkUnidad.Text = "Unidad";
            this.checkUnidad.UseVisualStyleBackColor = true;
            // 
            // numericPrecio
            // 
            this.numericPrecio.Location = new System.Drawing.Point(368, 26);
            this.numericPrecio.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericPrecio.Name = "numericPrecio";
            this.numericPrecio.Size = new System.Drawing.Size(55, 20);
            this.numericPrecio.TabIndex = 9;
            this.numericPrecio.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(323, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Precio:";
            // 
            // botonAgregarLinea
            // 
            this.botonAgregarLinea.Location = new System.Drawing.Point(562, 19);
            this.botonAgregarLinea.Name = "botonAgregarLinea";
            this.botonAgregarLinea.Size = new System.Drawing.Size(39, 30);
            this.botonAgregarLinea.TabIndex = 15;
            this.botonAgregarLinea.Text = " + ";
            this.botonAgregarLinea.UseVisualStyleBackColor = true;
            this.botonAgregarLinea.Click += new System.EventHandler(this.botonAgregarLinea_Click);
            // 
            // checkAfecto
            // 
            this.checkAfecto.AutoSize = true;
            this.checkAfecto.Checked = true;
            this.checkAfecto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAfecto.Location = new System.Drawing.Point(429, 27);
            this.checkAfecto.Name = "checkAfecto";
            this.checkAfecto.Size = new System.Drawing.Size(57, 17);
            this.checkAfecto.TabIndex = 12;
            this.checkAfecto.Text = "Afecto";
            this.checkAfecto.UseVisualStyleBackColor = true;
            // 
            // numericCantidad
            // 
            this.numericCantidad.DecimalPlaces = 1;
            this.numericCantidad.Location = new System.Drawing.Point(272, 25);
            this.numericCantidad.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericCantidad.Name = "numericCantidad";
            this.numericCantidad.Size = new System.Drawing.Size(45, 20);
            this.numericCantidad.TabIndex = 5;
            this.numericCantidad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(214, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Cantidad:";
            // 
            // textNombre
            // 
            this.textNombre.Location = new System.Drawing.Point(59, 25);
            this.textNombre.Name = "textNombre";
            this.textNombre.Size = new System.Drawing.Size(149, 20);
            this.textNombre.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Nombre:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gridResultados);
            this.groupBox1.Location = new System.Drawing.Point(12, 318);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(607, 170);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Productos";
            // 
            // gridResultados
            // 
            this.gridResultados.AllowUserToAddRows = false;
            this.gridResultados.AllowUserToDeleteRows = false;
            this.gridResultados.AllowUserToResizeColumns = false;
            this.gridResultados.AllowUserToResizeRows = false;
            this.gridResultados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridResultados.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.gridNombreProducto,
            this.gridCantidadProducto,
            this.gridPrecio,
            this.gridTotal,
            this.gridAfecto,
            this.umedida,
            this.gridEliminar});
            this.gridResultados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridResultados.Location = new System.Drawing.Point(3, 16);
            this.gridResultados.Name = "gridResultados";
            this.gridResultados.ReadOnly = true;
            this.gridResultados.RowHeadersWidth = 25;
            this.gridResultados.Size = new System.Drawing.Size(601, 151);
            this.gridResultados.TabIndex = 0;
            this.gridResultados.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridResultados_CellClick);
            // 
            // gridNombreProducto
            // 
            this.gridNombreProducto.DataPropertyName = "Nombre";
            this.gridNombreProducto.HeaderText = "Nombre";
            this.gridNombreProducto.Name = "gridNombreProducto";
            this.gridNombreProducto.ReadOnly = true;
            this.gridNombreProducto.Width = 230;
            // 
            // gridCantidadProducto
            // 
            this.gridCantidadProducto.DataPropertyName = "Cantidad";
            dataGridViewCellStyle7.Format = "N1";
            this.gridCantidadProducto.DefaultCellStyle = dataGridViewCellStyle7;
            this.gridCantidadProducto.HeaderText = "Cantidad";
            this.gridCantidadProducto.Name = "gridCantidadProducto";
            this.gridCantidadProducto.ReadOnly = true;
            this.gridCantidadProducto.Width = 55;
            // 
            // gridPrecio
            // 
            this.gridPrecio.DataPropertyName = "Precio";
            dataGridViewCellStyle8.Format = "N0";
            this.gridPrecio.DefaultCellStyle = dataGridViewCellStyle8;
            this.gridPrecio.HeaderText = "Precio";
            this.gridPrecio.Name = "gridPrecio";
            this.gridPrecio.ReadOnly = true;
            this.gridPrecio.Width = 65;
            // 
            // gridTotal
            // 
            this.gridTotal.DataPropertyName = "Total";
            dataGridViewCellStyle9.Format = "N0";
            this.gridTotal.DefaultCellStyle = dataGridViewCellStyle9;
            this.gridTotal.HeaderText = "Total";
            this.gridTotal.Name = "gridTotal";
            this.gridTotal.ReadOnly = true;
            this.gridTotal.Width = 70;
            // 
            // gridAfecto
            // 
            this.gridAfecto.DataPropertyName = "Afecto";
            this.gridAfecto.HeaderText = "Afecto";
            this.gridAfecto.Name = "gridAfecto";
            this.gridAfecto.ReadOnly = true;
            this.gridAfecto.Width = 45;
            // 
            // umedida
            // 
            this.umedida.DataPropertyName = "UnidadMedida";
            this.umedida.HeaderText = "Unidad";
            this.umedida.Name = "umedida";
            this.umedida.ReadOnly = true;
            this.umedida.Width = 45;
            // 
            // gridEliminar
            // 
            this.gridEliminar.HeaderText = "Elim.";
            this.gridEliminar.Name = "gridEliminar";
            this.gridEliminar.ReadOnly = true;
            this.gridEliminar.Width = 40;
            // 
            // numericCasoPrueba
            // 
            this.numericCasoPrueba.Enabled = false;
            this.numericCasoPrueba.Location = new System.Drawing.Point(215, 20);
            this.numericCasoPrueba.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericCasoPrueba.Name = "numericCasoPrueba";
            this.numericCasoPrueba.Size = new System.Drawing.Size(45, 20);
            this.numericCasoPrueba.TabIndex = 8;
            this.numericCasoPrueba.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelCasoPrueba
            // 
            this.labelCasoPrueba.AutoSize = true;
            this.labelCasoPrueba.Enabled = false;
            this.labelCasoPrueba.Location = new System.Drawing.Point(124, 23);
            this.labelCasoPrueba.Name = "labelCasoPrueba";
            this.labelCasoPrueba.Size = new System.Drawing.Size(85, 13);
            this.labelCasoPrueba.TabIndex = 7;
            this.labelCasoPrueba.Text = "Caso de prueba:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textGiroEmisor);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.textComunaEmisor);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.textDireccionEmisor);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.textRazonSocialEmisor);
            this.groupBox3.Controls.Add(this.textRUTEmisor);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(12, 71);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(607, 85);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Emisor";
            // 
            // textGiroEmisor
            // 
            this.textGiroEmisor.Location = new System.Drawing.Point(225, 50);
            this.textGiroEmisor.Name = "textGiroEmisor";
            this.textGiroEmisor.Size = new System.Drawing.Size(376, 20);
            this.textGiroEmisor.TabIndex = 24;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(186, 53);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Giro:";
            // 
            // textComunaEmisor
            // 
            this.textComunaEmisor.Location = new System.Drawing.Point(61, 50);
            this.textComunaEmisor.Name = "textComunaEmisor";
            this.textComunaEmisor.Size = new System.Drawing.Size(119, 20);
            this.textComunaEmisor.TabIndex = 22;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 53);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Comuna:";
            // 
            // textDireccionEmisor
            // 
            this.textDireccionEmisor.Location = new System.Drawing.Point(433, 24);
            this.textDireccionEmisor.Name = "textDireccionEmisor";
            this.textDireccionEmisor.Size = new System.Drawing.Size(168, 20);
            this.textDireccionEmisor.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(372, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Dirección:";
            // 
            // textRazonSocialEmisor
            // 
            this.textRazonSocialEmisor.Location = new System.Drawing.Point(225, 24);
            this.textRazonSocialEmisor.Name = "textRazonSocialEmisor";
            this.textRazonSocialEmisor.Size = new System.Drawing.Size(141, 20);
            this.textRazonSocialEmisor.TabIndex = 18;
            // 
            // textRUTEmisor
            // 
            this.textRUTEmisor.Location = new System.Drawing.Point(61, 24);
            this.textRUTEmisor.Name = "textRUTEmisor";
            this.textRUTEmisor.Size = new System.Drawing.Size(81, 20);
            this.textRUTEmisor.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(148, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Razón Social:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "RUT:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textGiroReceptor);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.textComunaReceptor);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.textDireccionReceptor);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.textRazonSocialReceptor);
            this.groupBox4.Controls.Add(this.textRUTReceptor);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Location = new System.Drawing.Point(12, 162);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(607, 85);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Receptor";
            // 
            // textGiroReceptor
            // 
            this.textGiroReceptor.Location = new System.Drawing.Point(225, 50);
            this.textGiroReceptor.Name = "textGiroReceptor";
            this.textGiroReceptor.Size = new System.Drawing.Size(376, 20);
            this.textGiroReceptor.TabIndex = 24;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(186, 53);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Giro:";
            // 
            // textComunaReceptor
            // 
            this.textComunaReceptor.Location = new System.Drawing.Point(61, 50);
            this.textComunaReceptor.Name = "textComunaReceptor";
            this.textComunaReceptor.Size = new System.Drawing.Size(119, 20);
            this.textComunaReceptor.TabIndex = 22;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 53);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Comuna:";
            // 
            // textDireccionReceptor
            // 
            this.textDireccionReceptor.Location = new System.Drawing.Point(433, 24);
            this.textDireccionReceptor.Name = "textDireccionReceptor";
            this.textDireccionReceptor.Size = new System.Drawing.Size(168, 20);
            this.textDireccionReceptor.TabIndex = 20;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(372, 27);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(55, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "Dirección:";
            // 
            // textRazonSocialReceptor
            // 
            this.textRazonSocialReceptor.Location = new System.Drawing.Point(225, 24);
            this.textRazonSocialReceptor.Name = "textRazonSocialReceptor";
            this.textRazonSocialReceptor.Size = new System.Drawing.Size(141, 20);
            this.textRazonSocialReceptor.TabIndex = 18;
            // 
            // textRUTReceptor
            // 
            this.textRUTReceptor.Location = new System.Drawing.Point(61, 24);
            this.textRUTReceptor.Name = "textRUTReceptor";
            this.textRUTReceptor.Size = new System.Drawing.Size(81, 20);
            this.textRUTReceptor.TabIndex = 4;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(146, 27);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 13);
            this.label14.TabIndex = 17;
            this.label14.Text = "Razón Social:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 27);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(33, 13);
            this.label15.TabIndex = 3;
            this.label15.Text = "RUT:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.comboTipo);
            this.groupBox5.Controls.Add(this.checkSetPruebas);
            this.groupBox5.Controls.Add(this.labelCasoPrueba);
            this.groupBox5.Controls.Add(this.numericCasoPrueba);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.numericFolio);
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(607, 53);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "General";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(396, 23);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(31, 13);
            this.label16.TabIndex = 16;
            this.label16.Text = "Tipo:";
            // 
            // comboTipo
            // 
            this.comboTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTipo.FormattingEnabled = true;
            this.comboTipo.Items.AddRange(new object[] {
            "BOLETA ELECTRÓNICA",
            "FACTURA ELECTRÓNICA",
            "FACTURA EXENTA ELECTRÓNICA"});
            this.comboTipo.Location = new System.Drawing.Point(433, 20);
            this.comboTipo.Name = "comboTipo";
            this.comboTipo.Size = new System.Drawing.Size(168, 21);
            this.comboTipo.TabIndex = 15;
            // 
            // checkSetPruebas
            // 
            this.checkSetPruebas.AutoSize = true;
            this.checkSetPruebas.Location = new System.Drawing.Point(9, 22);
            this.checkSetPruebas.Name = "checkSetPruebas";
            this.checkSetPruebas.Size = new System.Drawing.Size(111, 17);
            this.checkSetPruebas.TabIndex = 14;
            this.checkSetPruebas.Text = "¿Set de Pruebas?";
            this.checkSetPruebas.UseVisualStyleBackColor = true;
            this.checkSetPruebas.CheckedChanged += new System.EventHandler(this.checkSetPruebas_CheckedChanged);
            // 
            // GenerarDocumentoElectronico
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 542);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "GenerarDocumentoElectronico";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " ";
            this.Load += new System.EventHandler(this.GenerarBoletaElectronica_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericFolio)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPrecio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCantidad)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridResultados)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCasoPrueba)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericFolio;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button botonGenerar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkUnidad;
        private System.Windows.Forms.NumericUpDown numericPrecio;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button botonAgregarLinea;
        private System.Windows.Forms.CheckBox checkAfecto;
        private System.Windows.Forms.NumericUpDown numericCantidad;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textNombre;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView gridResultados;
        private System.Windows.Forms.NumericUpDown numericCasoPrueba;
        private System.Windows.Forms.Label labelCasoPrueba;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textDireccionEmisor;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textRazonSocialEmisor;
        private System.Windows.Forms.TextBox textRUTEmisor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textGiroEmisor;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textComunaEmisor;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textGiroReceptor;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textComunaReceptor;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textDireccionReceptor;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textRazonSocialReceptor;
        private System.Windows.Forms.TextBox textRUTReceptor;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboTipo;
        private System.Windows.Forms.CheckBox checkSetPruebas;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridNombreProducto;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridCantidadProducto;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridPrecio;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridTotal;
        private System.Windows.Forms.DataGridViewCheckBoxColumn gridAfecto;
        private System.Windows.Forms.DataGridViewTextBoxColumn umedida;
        private System.Windows.Forms.DataGridViewImageColumn gridEliminar;
    }
}