If DB_ID('DB_SE_EngineerWS') IS NOT NULL Begin
    Use master; 
    Drop Database DB_SE_EngineerWS;
End
Go

Create Database DB_SE_EngineerWS;
Go

Use DB_SE_EngineerWS;
Go

Create Type string_short From varchar(500);
Create Type string_medium From varchar(1000);
Create Type string_long From varchar(5000);
Create Type string_max From varchar(6500);
Go

Create Type export_product As Table (
	product			string_short NOT NULL,
	count			int NOT NULL,
	Check (count > 0)
);
Go

Create Table [dbo].Roles (
	code			int NOT NULL unique identity(1, 1),
	name			string_short NOT NULL,
	Check (len(name) > 3),
	Constraint PK_Roles_code Primary key (code)
);
Go

Create Table [dbo].Users (
	login			string_short NOT NULL unique,
	password		string_short NOT NULL,
	role			int NOT NULL,
	name			string_short NOT NULL,
	Check (len(login) > 2),
	Check (len(password) > 5),
	Check (len(name) > 1),
	Constraint FK_Users_to_Roles Foreign key (role) References Roles (code)
		On delete cascade
		On update cascade
);
Go

Create Table [dbo].Family (
	name			string_short NOT NULL unique,
	Check (len(name) > 0),
	Constraint PK_Family_name Primary key (name)
);
Go

Create Table [dbo].Product (
	name			string_short NOT NULL unique,
	family			string_short NULL,
	Check (len(name) > 0),
	Constraint PK_Product_name Primary key (name),
	Constraint FK_Product_to_Family Foreign key (family) References Family (name)
		On delete set NULL
		On update cascade
);
Go

Create Table [dbo].Component (
	name			string_short NOT NULL,
	code			string_short NOT NULL unique,
	grade			string_short NOT NULL,
	thickness		decimal(3, 1) NOT NULL,
	folds			int NOT NULL default(0),
	weightKG		decimal(5, 2) NOT NULL,
	note			string_medium NOT NULL default(''),
	material		string_short NOT NULL default('собственный'),
	Check (len(name) > 0 and len(code) > 0 and len(grade) > 0),
	Check (thickness > 0 and weightKG > 0),
	Check (len(material) > 0),
	Constraint PK_Component_code Primary key (code)
);
Go

Create Table [dbo].Structure (
	id				int NOT NULL unique identity (1, 1),
	product			string_short NOT NULL,
	component		string_short NOT NULL,
	count			int NOT NULL default(1), 
	Constraint PK_Structure_id Primary key (id),
	Constraint FK_Structure_to_Product Foreign key (product) References Product (name)
		On delete cascade
		On update cascade,
	Constraint FK_Structure_to_Component Foreign key (component) References Component (code)
		On delete cascade
		On update cascade
);
Go

Create Table [dbo].Component_File (
	id				int NOT NULL unique identity(1, 1),
	component		string_short NULL,
	localFilePath	string_long NOT NULL default(''),
	Constraint PK_Component_id Primary key (id),
	Constraint FK_to_Component Foreign key (component) References Component (code)
		On delete set NULL
		On update cascade
);
Go