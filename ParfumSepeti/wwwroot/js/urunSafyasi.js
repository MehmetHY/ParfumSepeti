const adetElm = document.getElementById('adet');
const modelElm = document.getElementById('model-elm');
const sepeteGitDiv = document.getElementById('sepete-git-div');
const sepeteEkleDiv = document.getElementById('sepete-ekle-div');
const istekBtn = document.getElementById('istek-btn');
const urunId = modelElm.dataset.urunId;
sepetButonunuGuncelle();
istekButonunuGuncelle();
console.log('\n\n\n\n\n\n\n\n');

async function sepetButonunuGuncelle() {
    const sepette = await sepetteMi();
    console.log('Sepette mi? ', sepette);

    if (sepette) {
        if (sepeteGitDiv.classList.contains('hide'))
            sepeteGitDiv.classList.remove('hide');

        if (!sepeteEkleDiv.classList.contains('hide'))
            sepeteEkleDiv.classList.add('hide');
    } else {
        if (!sepeteGitDiv.classList.contains('hide'))
            sepeteGitDiv.classList.add('hide');

        if (sepeteEkleDiv.classList.contains('hide'))
            sepeteEkleDiv.classList.remove('hide');
    }
}

async function sepetteMi() {
    let sepette = false;

    await fetch(`/Magaza/SepetteMi/${urunId}`)
        .then(response => response.json())
        .then(data => {
            sepette = data;
        })
        .catch(err => console.log('sepetteMi() -> ', err));

    return sepette;
}

function sepeteEkle() {
    fetch('/Magaza/SepeteEkle', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            urunId: parseInt(urunId),
            adet: parseInt(adetElm.innerHTML),
        }),
    })
        .then(response => {
            if (response.ok) {
                sepetButonunuGuncelle();
            } else {
                throw new Error(response.status, response.statusText);
            }
        })
        .catch(err => console.error('sepeteEkle() -> ', err));
}

function sepeteGit() {
    location.assign('/Magaza/Sepet');
}

function arttir() {
    let adet = parseInt(adetElm.innerHTML);
    ++adet;

    if (adet > 10) {
        adet = 10;
    }

    adetElm.innerHTML = adet;
}

function azalt() {
    let adet = parseInt(adetElm.innerHTML);
    --adet;

    if (adet < 1) {
        adet = 1;
    }

    adetElm.innerHTML = adet;
}

async function girisYapildiMi() {
    let result = false;

    await fetch('/Kullanici/GirisYapildiMi')
        .then(response => response.json())
        .then(data => {
            result = data;
        })
        .catch(err => console.error('girisYapildiMi() -> ', err));

    return result;
}

async function onIstekPressed() {
    istekBtn.disable = true;

    if (!(await girisYapildiMi())) {
        location.assign(
            `/Kullanici/Giris?ReturnUrl=%2FMagaza%2FUrun%2F${urunId}`
        );

        return;
    }

    if (await istekListesindeMi()) await istekListesindenKaldir();
    else  await istekListesineEkle();

    istekBtn.disable = false;
}

async function istekListesineEkle() {
    fetch('/Kullanici/IstekListesineEkle', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            urunId: urunId,
        }),
    })
        .then(response => {
            if (response.ok) istekButonunuGuncelle();
        })
        .catch(err => console.error('istekListesineEkle() -> ', err));
}

async function istekListesindenKaldir() {
    fetch('/Kullanici/IstekListesindenKaldir', {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            urunId: urunId,
        }),
    })
        .then(response => {
            if (response.ok) istekButonunuGuncelle();
        })
        .catch(err => console.error('istekListesindenKaldir() -> ', err));
}

async function istekListesindeMi() {
    let result = false;

    await fetch(`/Kullanici/IstekListesindeMi/${urunId}`)
        .then(response => response.json())
        .then(data => (result = data))
        .catch(err => console.error('istekListesindeMi() -> ', err));

    return result;
}

async function istekButonunuGuncelle() {
    if (await istekListesindeMi()) {
        istekBtn.classList.remove('btn2-danger');
        istekBtn.classList.add('btn-danger');
        istekBtn.innerHTML = 'İstek Listesinden Kaldır';
        console.log('istek listesinde');
    } else {
        istekBtn.classList.remove('btn-danger');
        istekBtn.classList.add('btn2-danger');
        istekBtn.innerHTML = 'İstek Listesine Ekle';
        console.log('istek listesinde yok');
    }
}
