Use DB_SE_EngineerWS;
Go

Create Procedure [dbo].GetFullExportTable
	@ProductsList export_product readonly
As Begin
	/* формирование таблицы совсеми данными на внесение в excel */
	Select 'await';
End
Go

Create procedure [dbo].GetProductsListByComponent
	@Component string_short
As Begin
	Select	VProductList.*
	From	VProductList
	Inner Join VProductComponentsList on VProductComponentsList.product = VProductList.name
	Where	VProductComponentsList.name = @Component
End
Go

Create Procedure [dbo].GetProductsListByFamily
	@Family string_short
As Begin
	Select	*
	From	VProductList
	Where	VProductList.family = @Family
End
Go

Create Procedure [dbo].GetComponentsListByProduct
	@Product string_short
As Begin
	Select	*
	From	VProductComponentsList
	Where	VProductComponentsList.product = @Product
End
Go

Create Procedure [dbo].GetFilesListByComponent
	@Component string_short
As Begin
	Select	*
	From	Component_File
	Where	Component_File.component = @Component
End
Go

Create Procedure [dbo].RenameFamily
	@Fiamily string_short,
	@NewName string_short
As Begin
	Update	Family
	Set		Family.name = @NewName
	Where	Family.name = @Fiamily
End
Go

Create Procedure [dbo].RenameProduct
	@Product string_short,
	@NewName string_short
As Begin
	Update	Product
	Set		Product.name = @NewName
	Where	Product.name = @Product
End
Go

Create Procedure [dbo].RenameComponent
	@Component string_short,
	@NewName string_short
As Begin
	Update	Component
	Set		Component.name = @NewName
	Where	Component.name = @Component
End
Go

Create Procedure [dbo].ChangeFilePath
	@id string_short,
	@NewPath string_short
As Begin
	Update	Component_File
	Set		Component_File.localFilePath = @NewPath
	Where	Component_File.id = @id
End
Go