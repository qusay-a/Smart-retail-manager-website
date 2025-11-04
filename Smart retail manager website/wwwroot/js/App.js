function App() {
    const [products, setProducts] = React.useState([]);

    React.useEffect(() => {
        fetch("/api/products")
            .then(res => res.json())
            .then(data => setProducts(data))
            .catch(err => console.error("Error fetching products:", err));
    }, []);

    return (
        <div className="container mt-4">
            <h3>Available Products</h3>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Category</th>
                        <th>Price</th>
                        <th>Stock</th>
                    </tr>
                </thead>
                <tbody>
                    {products.map(p => (
                        <tr key={p.productID}>
                            <td>{p.productID}</td>
                            <td>{p.name}</td>
                            <td>{p.category}</td>
                            <td>{p.unitPrice}</td>
                            <td>{p.quantityInStock}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(<App />);
