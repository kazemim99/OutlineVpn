using OutlineVpn;

var api = new OutlineApi("https://13.232.11.178:44751/w7BTeKeVYCIwb8jPIu94eA"); 

var data = api.CreateKey(); // Create new key
api.RenameKey(data.Id, "Test_name"); // Rename new key

var data2 = api.GetTransferredData(); // Get all transferred data
var data3 = api.GetKeys(); // Get all transferred data

var fff = data2.FirstOrDefault(a => a.Id == 12).UsedBytes / Math.Pow(1024, 2);
Console.WriteLine(data2.FirstOrDefault(k => k.Id == 0).UsedBytes); // Print used traffic with id 0