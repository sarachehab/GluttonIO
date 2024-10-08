
//=======================================================
//  This code is generated by Terasic System Builder
//=======================================================

module DE10_LITE_Golden_Top(

	//////////// CLOCK //////////
	input 		          		ADC_CLK_10,
	input 		          		MAX10_CLK1_50,
	input 		          		MAX10_CLK2_50,

	//////////// SDRAM //////////
	output		    [12:0]		DRAM_ADDR,
	output		     [1:0]		DRAM_BA,
	output		          		DRAM_CAS_N,
	output		          		DRAM_CKE,
	output		          		DRAM_CLK,
	output		          		DRAM_CS_N,
	inout 		    [15:0]		DRAM_DQ,
	output		          		DRAM_LDQM,
	output		          		DRAM_RAS_N,
	output		          		DRAM_UDQM,
	output		          		DRAM_WE_N,

	//////////// SEG7 //////////
	output		     [7:0]		HEX0,
	output		     [7:0]		HEX1,
	output		     [7:0]		HEX2,
	output		     [7:0]		HEX3,
	output		     [7:0]		HEX4,
	output		     [7:0]		HEX5,

	//////////// KEY //////////
	input 		     [1:0]		KEY,

	//////////// LED //////////
	output		     [9:0]		LEDR,

	//////////// SW //////////
	input 		     [9:0]		SW,

	//////////// VGA //////////
	output		     [3:0]		VGA_B,
	output		     [3:0]		VGA_G,
	output		          		VGA_HS,
	output		     [3:0]		VGA_R,
	output		          		VGA_VS,

	//////////// Accelerometer //////////
	output		          		GSENSOR_CS_N,
	input 		     [2:1]		GSENSOR_INT,
	output		          		GSENSOR_SCLK,
	inout 		          		GSENSOR_SDI,
	inout 		          		GSENSOR_SDO,

	//////////// Arduino //////////
	inout 		    [15:0]		ARDUINO_IO,
	inout 		          		ARDUINO_RESET_N,

	//////////// GPIO, GPIO connect to GPIO Default //////////
	inout 		    [35:0]		GPIO
);


//=======================================================
//  REG/WIRE declarations
//=======================================================
	
	wire [31:0] accel_x_raw, accel_y_raw;
	wire [31:0] accel_x_filter, accel_y_filter;
	wire clk_3200;
	
	wire debounced_key1;
	wire debounced_key0;
	
	
//=======================================================
//  Structural coding
//=======================================================
	
	// 50 MHz -> 3.2kHz (factor of 15625)
	clk_divider #(15625) divider(
		.clk_in(MAX10_CLK1_50),
		.rst(1'b0),
		.clk_out(clk_3200)
	);
	
	// FIR filter for x-axis
	fir fir_x(
		.clk(clk_3200),
		.in(accel_x_raw),
		.out(accel_x_filter)
	);
	
	// FIR filter for y-axis
	fir fir_y(
		.clk(clk_3200),
		.in(accel_y_raw),
		.out(accel_y_filter)
	);
	
	debounce debounce_key1(
		.clk(MAX10_CLK1_50),
		.in(KEY[1]),
		.out(debounced_key1)
	);
	
	
	debounce debounce_key0(
		.clk(MAX10_CLK1_50),
		.in(KEY[0]),
		.out(debounced_key0)
	);
	
	
	nios_accelerometer u0(
		.clk_clk(MAX10_CLK1_50),
		.led_external_connection_export(LEDR[9:0]),
		.reset_reset_n(1'b1),
		.accelerometer_spi_0_external_interface_I2C_SDAT(GSENSOR_SDI),
		.accelerometer_spi_0_external_interface_I2C_SCLK(GSENSOR_SCLK),
		.accelerometer_spi_0_external_interface_G_SENSOR_CS_N (GSENSOR_CS_N),
		.accelerometer_spi_0_external_interface_G_SENSOR_INT(GSENSOR_INT[1]),
		.button_external_connection_export({debounced_key1, debounced_key0}),
		.switch_external_connection_export(SW[9:0]),
		.filter_x_external_connection_export(accel_x_filter),                 
		.filter_y_external_connection_export(accel_y_filter),
		.raw_x_external_connection_export(accel_x_raw),
		.raw_y_external_connection_export(accel_y_raw)
	);



endmodule
