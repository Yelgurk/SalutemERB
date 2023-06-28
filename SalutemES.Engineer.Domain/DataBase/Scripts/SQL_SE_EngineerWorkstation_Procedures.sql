Use DB_SE_EngineerWS;
Go

Create Procedure [dbo].GetExportTable
	@ProductsList export_product readonly
As Begin
	Select  Component.name,
		Component.code,
		sum(Structure.count * TT.count) as 'count',
		Component.grade,
		Component.thickness,
		Replace(Component.folds, 0, '') as 'folds',
		Component.weightKG,
		sum(Component.weightKG * Structure.count * TT.count) as 'totalKG',
		Component.note,
		Component.material
	From Component
	Inner Join Structure on Structure.component = Component.name
	Inner Join @ProductsList as TT on TT.product = Structure.product
	Group by	Component.name,
			Component.code,
			Component.grade,
			Component.thickness,
			Component.folds,
			Component.weightKG,
			Component.note,
			Component.material;
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
	Where	Component_File.id = (Select cast(@id as int))
End
Go