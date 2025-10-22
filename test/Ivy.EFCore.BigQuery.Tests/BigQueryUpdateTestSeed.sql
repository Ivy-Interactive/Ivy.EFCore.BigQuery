INSERT INTO `Products` (Id, Name, Price, Category, CreatedDate, IsActive, SupplierId, Description) VALUES
(1, 'Laptop', 1200.00, 'Electronics', '2023-01-15 10:30:00', true, 101, 'High-performance laptop'),
(2, 'Book', 25.50, 'Books', '2023-02-20 14:00:00', true, 102, 'A great book'),
(3, 'Keyboard', 75.00, 'Electronics', '2023-03-10 09:00:00', true, 101, 'Mechanical keyboard'),
(4, 'Mouse', 25.00, 'Electronics', '2023-01-25 11:00:00', false, 101, 'Ergonomic mouse'),
(5, 'T-shirt', 15.00, 'Clothing', '2023-04-05 16:45:00', true, 103, 'Cotton t-shirt');

INSERT INTO `Orders` (OrderId, CustomerId, OrderDate, TotalAmount, Status, ShippingAddress) VALUES
(1, 1, '2023-05-01 12:15:00', 1275.50, 'Shipped', '123 Main St'),
(2, 2, '2023-05-03 18:45:00', 25.00, 'Pending', '456 Oak Ave'),
(3, 1, '2023-05-04 09:30:00', 75.00, 'Delivered', '123 Main St');

INSERT INTO `Customers` (Id, Name, Email, LastOrderDate, IsVip, Region) VALUES
(1, 'John Doe', 'john.doe@example.com', '2023-05-04 09:30:00', false, 'West'),
(2, 'Jane Smith', 'jane.smith@example.com', '2023-05-03 18:45:00', true, 'East');
