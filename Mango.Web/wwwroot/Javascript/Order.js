var dataTable;
function loadDataaTable() {
    dataTable = $('#tblData').DataTable({

        "ajax": { url: "order/getall" },
        "columns": [
            { data: "orderHeaderid", width: "5%" },
            { data: "email", width: "25%" },
            { data: "status", width: "10%" },
            { data: "totalPrice", width: "10%" },
            { data: "customerName", width: "10%" },

        ],


    });
}