using OutlineVpn;

var api = new OutlineApi("https://13.232.11.178:44751/w7BTeKeVYCIwb8jPIu94eA"); 

var data = api.CreateKey(10); // Create new key
api.RenameKey(data.Id, "Test_name"); // Rename new key


Console.WriteLine(data2.FirstOrDefault(k => k.Id == 0).UsedBytes); // Print used traffic with id 0