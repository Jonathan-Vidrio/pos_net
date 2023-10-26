document.addEventListener('DOMContentLoaded', function() {
    const searchInput = document.getElementById('productSearchInput');
    const productCodeInput = document.getElementById('productCode');
    const cartBody = document.querySelector('#cartBody');
    const cartTotal = document.getElementById('cartTotal');
    const totalSale = document.getElementById('totalSale');
    const amountReceived = document.getElementById('amountReceived');
    const changeAmount = document.getElementById('changeAmount');
    const detailsForm = document.getElementById('detailsForm');
    const addProductButton = document.getElementById('addProduct');
    const completeSaleButton = document.getElementById('completeSale');
    const confirmSaleButton = document.getElementById('confirmSale');
    const searchProductButton = document.getElementById('searchProductButton');
    const saveQuantityButton = document.getElementById('saveQuantity');
    const productsModal = $('#productsModal');
    const completionModal = $('#completionModal');
    const quantityModal = $('#quantityModal');

    function populateProducts(products) {
        const tbody = document.getElementById('productsList');
        tbody.innerHTML = '';  // Limpiar tabla

        for (let product of products) {
            let row = tbody.insertRow();
            row.insertCell(0).innerText = product.code;
            row.insertCell(1).innerText = product.description;
            row.insertCell(2).innerText = product.price;

            let selectCell = row.insertCell(3);
            let selectButton = document.createElement('button');
            selectButton.innerText = 'Seleccionar';
            selectButton.classList.add('btn', 'btn-primary');
            selectButton.onclick = function() {
                productCodeInput.value = product.code;
                addProductToCart();
                productsModal.modal('hide');
            };
            selectCell.appendChild(selectButton);
        }
    }

    async function addProductToCart() {
        const productCode = productCodeInput.value;

        try {
            const response = await fetch(`/Product/GetByCode/${productCode}`);
            if (!response.ok) {
                throw new Error('No se pudo obtener el producto');
            }

            const product = await response.json();

            if (!product) {
                alert('Producto no encontrado. Verifica el código ingresado.');
                productsModal.modal('show');
                return;
            }

            let existingRow = [...cartBody.querySelectorAll('tr')].find(row => row.cells[0].innerText === productCode);
            if (existingRow) {
                const quantitySpan = existingRow.cells[3].querySelector('span');
                quantitySpan.innerText = parseInt(quantitySpan.innerText) + 1;
                updateSubtotal(existingRow);
                updateTotal();
                return;
            }

            addToCart(product);
            updateTotal();
        } catch (error) {
            console.error(error);
            alert('Error al agregar producto.');
        }
    }

    function updateSubtotal(row) {
        const price = parseFloat(row.cells[2].innerText);
        const quantity = parseInt(row.cells[3].querySelector('span').innerText);
        row.cells[4].innerText = (price * quantity).toFixed(2);
    }

    function updateTotal() {
        const rows = [...cartBody.querySelectorAll('tr')];
        const total = rows.reduce((acc, row) => acc + parseFloat(row.cells[4].innerText), 0);
        cartTotal.innerText = `$${total.toFixed(2)}`;
    }

    function addToCart(product) {
        let newRow = cartBody.insertRow();

        newRow.insertCell(0).innerText = product.code;
        newRow.insertCell(1).innerText = product.description;
        newRow.insertCell(2).innerText = product.price;
        newRow.insertCell(3).innerHTML = '<span>1</span>';

        let subtotal = parseFloat(product.price).toFixed(2);
        newRow.insertCell(4).innerText = subtotal;

        let deleteCell = newRow.insertCell(5);
        let deleteButton = document.createElement('button');
        deleteButton.innerText = 'Eliminar';
        deleteButton.classList.add('btn', 'btn-danger');
        deleteButton.onclick = function() {
            cartBody.removeChild(newRow);
            updateTotal();
        };
        deleteCell.appendChild(deleteButton);
        productCodeInput.focus();
    }

    async function searchProducts() {
        const query = searchInput.value;
        if (!query) return;

        try {
            const response = await fetch(`/Product/Search/${query}`);
            if (!response.ok) {
                throw new Error('Error al buscar productos');
            }

            const products = await response.json();
            populateProducts(products);
        } catch (error) {
            console.error(error);
            alert('Error al buscar productos.');
        }
    }
    
    const confirmSale = () => {
        // Verificación de valores
        const rawTotal = cartTotal.innerText.replace('$', '');
        const rawReceived = amountReceived.value;

        if (!isNumeric(rawTotal) || !isNumeric(rawReceived)) {
            console.error("Total or received amount is not numeric", rawTotal, rawReceived);
            alert('Error: Datos inválidos.');
            return;
        }

        function updateQuantityFunction() {
            const newQuantity = document.getElementById('newQuantity').value;
            clickedRow.cells[3].querySelector('span').innerText = newQuantity;
            updateSubtotal(clickedRow);
            updateTotal();
            quantityModal.modal('hide');
        }

        const Total = parseFloat(rawTotal);
        const CashReceived = parseFloat(rawReceived);
        const Change = CashReceived - Total;

        // Verificar si Change es NaN
        if (isNaN(Change)) {
            console.error("Change calculation resulted in NaN", Total, CashReceived, Change);
            alert('Error: No se pudo calcular el cambio.');
            return;
        }

        const sale = {
            Date: new Date().toISOString(),
            Client: "Público en General", // Aquí puedes agregar una forma de obtener un cliente específico si es necesario
            Total: Total,
            CashReceived: CashReceived,
            Change: Change,
            SaleDetails: [...cartBody.querySelectorAll('tr')].map(row => ({
                Code: row.cells[0].innerText,
                Quantity: parseInt(row.cells[3].querySelector('span').innerText),
                UnitPrice: parseFloat(row.cells[2].innerText),
                Subtotal: parseFloat(row.cells[2].innerText) * parseInt(row.cells[3].querySelector('span').innerText)
            })),
            Status: true
        };

        console.log(JSON.stringify(sale));

        fetch('/Sale/Register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(sale)
        }).then(response => {
            if (!response.ok) {
                throw new Error('Error al registrar venta');
            }
        }).then(data => {
            alert('Venta registrada exitosamente');
            cartBody.innerHTML = '';
            cartTotal.innerText = '$0.00';
            amountReceived.value = '';
            changeAmount.innerText = '$0.00';
            productCodeInput.focus();
            completionModal.modal('hide');
        }).catch(error => {
            console.log(error);
            alert('Error al registrar la venta.');
        });
    }

    function isNumeric(value) {
        return !isNaN(parseFloat(value)) && isFinite(value);
    }

    productCodeInput.focus();

    productsModal.on('show.bs.modal', async function() {
        try {
            const response = await fetch('/Product/GetAll');
            if (!response.ok) {
                throw new Error('Error al obtener productos');
            }

            const products = await response.json();
            populateProducts(products);
        } catch (error) {
            console.error(error);
            alert('Error al cargar los productos.');
        }
    });

    productsModal.on('shown.bs.modal', function() {
        searchInput.focus();
    });

    searchProductButton.addEventListener('click', searchProducts);

    cartBody.addEventListener('dblclick', function(event) {
        if (event.target.tagName === 'TD') {
            quantityModal.modal('show');
            const clickedRow = event.target.closest('tr');

            document.getElementById('updateQuantityButton').onclick = function() {
                const newQuantity = document.getElementById('newQuantity').value;
                clickedRow.cells[3].querySelector('span').innerText = newQuantity;
                saveQuantityButton.removeEventListener('click', updateQuantityFunction);
                saveQuantityButton.addEventListener('click', updateQuantityFunction);
                updateSubtotal(clickedRow);
                updateTotal();
                quantityModal.modal('hide');
            };
        }
    });
    
    completeSaleButton.addEventListener('click', completionModal.modal.bind(completionModal, 'show'));

    document.addEventListener('keydown', function(event) {
        if (event.key === 'F8') {
            productsModal.modal('show');
        }

        if (event.key === 'Escape' || event.key === 'Esc') {
            completionModal.modal('show');
        }

        if (event.key === 'Enter' && completionModal.hasClass('show')) {
            confirmSale();
        }

        if (event.key === 'Enter' && productsModal.hasClass('show')) {
            searchProducts();
        }
    });

    completionModal.on('hidden.bs.modal', function() {
        productCodeInput.focus();
    });
    
    completionModal.on('shown.bs.modal', function() {
        amountReceived.focus();
        totalSale.value = cartTotal.innerText.replace('$', '');
    });

    addProductButton.addEventListener('click', addProductToCart);

    detailsForm.addEventListener('submit', function(event) {
        event.preventDefault();
        addProductToCart();
    });

    amountReceived.addEventListener('input', function() {
        const amount = parseFloat(amountReceived.value);
        const total = parseFloat(cartTotal.innerText.replace('$', ''));
        const change = amount - total;

        if (change < 0 || isNaN(change)) {
            changeAmount.value = '0.00';
            confirmSaleButton.disabled = true; // Deshabilitar el botón de finalizar venta
        } else {
            changeAmount.value = change.toFixed(2);
            confirmSaleButton.disabled = false; // Habilitar el botón de finalizar venta
        }
    });

    confirmSaleButton.addEventListener('click', confirmSale);

});