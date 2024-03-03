window.onload = function () {

    document.querySelector('#search-input')
        .addEventListener('keyup', async (e) => {

            e.preventDefault();

            const form = new FormData(e.currentTarget.form);

            const headerOptions = {
                'method': 'POST',
                'headers': {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                'body': convertFormToJSON(form)
            }


            try {

                const resp = await fetch('/search', headerOptions);
                const data = await resp.json();

                

                if (data[0].clientName) {

                    let invoicesTemplate = `
                        <thead>
                            <tr>
                                <th>Facture ID</th>
                                <th>Facture Date</th>
                                <th>Client Nom</th>
                                <th>Fournisseur Nom</th>
                                <th>Montant TTC</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                    `;

                    data.map(x => {
                        invoicesTemplate += `
                           <tr>
                                <td class="text-start">${x.invoiceID}</td>
                                <td class="text-start">${x.invoiceDate}</td>
                                <td class="text-start">${x.clientName}</td>
                                <td class="text-start">${x.supplierName}</td>
                                <td class="text-start">${x.itemsPriceAfterTax}.00</td>
                                <td>
                                    <a class="btn btn-sm btn-success" href="/Invoice/Detail?invoiceId=${x.invoiceID}">Afficher</a>
                                </td>
                            </tr>
                        ` ;
                    })

                    invoicesTemplate += '</tbody>';

                    document.querySelector('#tbl_invoices')
                        .innerHTML = invoicesTemplate;

                } else if (data[0].itemLibelle) {

                    let itemTemplate = `
                            <thead>
                                <tr>
                                    <th>Item ID</th>
                                    <th>Item Libelle</th>
                                    <th>Unité d’Item</th>
                                    <th>Quantité d’item</th>
                                    <th>Prix d’item</th>
                                    <th>Taxe d’item</th>
                                </tr>
                            </thead>
                            <tbody>
                    `;

                    data.map(x => {
                        itemTemplate += `
                            <tr>
                                <td class="text-start">${x.itemID}</td>
                                <td class="text-start">${x.itemLibelle}</td>
                                <td class="text-start">${x.itemUnit}</td>
                                <td class="text-start">${x.itemQuantity}</td>
                                <td class="text-start">${x.itemPrice}.00</td>
                                <td class="text-start">${x.itemTax}.00</td>
                            </tr>
                        `;
                    })

                    itemTemplate += '</tbody>';

                    document.querySelector('#tbl_invoices')
                        .innerHTML = itemTemplate;
                }

            } catch (error) {
                console.log(error);
            }

        })
}

function convertFormToJSON(form) {
    let formObject = {};

    for (let key of form.keys()) {
        formObject[key] = form.get(key);
    }

    return JSON.stringify(formObject);
}