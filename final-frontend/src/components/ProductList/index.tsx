import React, { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import { Product } from "../../models/types";

export default function ProductList() {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("/updatesHub")
      .build();

    connection.on("ProductAdded", (product: Product) => {
      setProducts([...products, product]);
    });

    connection.start().catch((err) => console.error(err.toString()));
  }, [products]);

  return (
    <div>
      <h2>Products</h2>
      <ul>
        {products.map((product) => (
          <li key={product.id}>{product.name}</li>
        ))}
      </ul>
    </div>
  );
}