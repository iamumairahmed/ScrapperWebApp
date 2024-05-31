using ScrapperWebApp.Data;
using System.ComponentModel;
using System.Data;

namespace ScrapperWebApp.Utility
{
    public class Helper
    {
        public static DataTable ConvertToDataTable<T>(List<T> list)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Get all public properties of type T
                var properties = typeof(T).GetProperties();

                // Create columns in DataTable based on properties of T
                foreach (var property in properties)
                {
                    Type propertyType = property.PropertyType;

                    // Check if property type is a list
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        // Add a string column to represent the list as a comma-separated string
                        dataTable.Columns.Add(property.Name, typeof(string));
                    }
                    else
                    {
                        dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(propertyType) ?? propertyType);
                    }
                }

                // Add rows to DataTable
                foreach (var item in list)
                {
                    DataRow dataRow = dataTable.NewRow();
                    foreach (var property in properties)
                    {
                        object value = property.GetValue(item);

                        // Check if property is a list
                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            // Convert list to comma-separated string
                            if (value is IEnumerable<object> listValue)
                            {
                                dataRow[property.Name] = string.Join(", ", listValue);
                            }
                            else
                            {
                                dataRow[property.Name] = DBNull.Value;
                            }
                        }
                        else
                        {
                            dataRow[property.Name] = value ?? DBNull.Value;
                        }
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Datatable Coversion Error: " + ex.ToString());
            }
            return dataTable;
        }

        public static DataTable ConvertToDataTableExport<T>(List<T> list, int maxPhoneCount)
        {
            DataTable dataTable = new DataTable();
            DataTable table = new DataTable();

            List<string> columns = ["NoCnpj", "CdRzsocial", "CdEmail", "Telefones"];
            try
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

                foreach (PropertyDescriptor prop in properties)
                {
                    if (columns.Contains(prop.Name))
                    {
                        if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                        {
                            if (maxPhoneCount == 0)
                            {
                                table.Columns.Add($"{prop.Name}1", typeof(string));
                            }
                            else
                            {
                                // If the property is a list, we need to add multiple columns for each item
                                for (int i = 1; i <= maxPhoneCount; i++)
                                {
                                    table.Columns.Add($"{prop.Name}{i}", typeof(string));
                                }
                            }
                        }
                        else
                        {
                            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        }
                    }
                }

                foreach (T item in list)
                {

                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        if (columns.Contains(prop.Name))
                        {
                            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                            {
                                // Add each item in the list to the corresponding column
                                var listt = prop.GetValue(item) as IEnumerable<Telefone>;
                                object value = prop.GetValue(item);

                                if (listt != null)
                                {
                                    int index = 1;
                                    foreach (var listItem in listt)
                                    {
                                        row[$"{prop.Name}{index++}"] = listItem.NoFone;
                                    }
                                }
                            }
                            else
                            {
                                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                            }
                        }
                    }
                    table.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Datatable Coversion Error: " + ex.ToString());
            }
            return table;
        }
    }
}
