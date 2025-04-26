namespace Project.Pages.Client;
public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        SetActiveButton(btnHome); // ��� �������� ����� �������������� "�������"
    }

    private void SetActiveButton(ImageButton activeButton)
    {
        // ����� ���� ������ � �����
        btnHome.Source = "home_gray.png";
        btnCatalog.Source = "catalog_gray.png";
        btnFavourite.Source = "favourite_grey.png";
        btnBasket.Source = "basket_grey.png";
        btnProfile.Source = "profile_grey.png";

        lblHome.TextColor = Colors.Gray;
        lblCatalog.TextColor = Colors.Gray;
        lblFavourite.TextColor = Colors.Gray;
        lblBasket.TextColor = Colors.Gray;
        lblProfile.TextColor = Colors.Gray;

        // ��������� ��������� ������
        if (activeButton == btnHome)
        {
            btnHome.Source = "home_green.png";
            lblHome.TextColor = Colors.Green;
        }
        else if (activeButton == btnCatalog)
        {
            btnCatalog.Source = "catalog_green.png";
            lblCatalog.TextColor = Colors.Green;
        }
        else if (activeButton == btnFavourite)
        {
            btnFavourite.Source = "favourite_green.png";
            lblFavourite.TextColor = Colors.Green;
        }
        else if (activeButton == btnBasket)
        {
            btnBasket.Source = "basket_green.png";
            lblBasket.TextColor = Colors.Green;
        }
        else if (activeButton == btnProfile)
        {
            btnProfile.Source = "profile_green.png";
            lblProfile.TextColor = Colors.Green;
        }
    }

    private void OnHomeClicked(object sender, EventArgs e)
    {
        SetActiveButton(btnHome);
        // ���������, ���� ����
    }

    private void OnCatalogClicked(object sender, EventArgs e)
    {
        SetActiveButton(btnCatalog);
        // ���������
    }

    private void OnFavouriteClicked(object sender, EventArgs e)
    {
        SetActiveButton(btnFavourite);
        // ���������
    }

    private void OnBasketClicked(object sender, EventArgs e)
    {
        SetActiveButton(btnBasket);
        // ���������
    }

    private void OnProfileClicked(object sender, EventArgs e)
    {
        SetActiveButton(btnProfile);
        // ���������
    }
}
