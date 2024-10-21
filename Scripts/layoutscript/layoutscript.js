
        function toggleMenu() {
            var mobileMenu = document.querySelector('.mobile-menu');
            mobileMenu.classList.toggle('open');
        }


        // JavaScript to handle dropdown toggle
        $(document).ready(function () {
            // Hide dropdown menu initially
            $('#dropdownMenu').hide();

            // Toggle dropdown menu when clicking on dropdown toggle
            $('#dropdownMenuLink').on('click', function (e) {
                e.preventDefault();
                $('#dropdownMenu').toggle();
            });

            // Close dropdown menu when clicking outside of it
            $(document).on('click', function (e) {
                if (!$(e.target).closest('.dropdown').length) {
                    $('#dropdownMenu').hide();
                }
            });
        });

        // JavaScript to handle form submission for logout
        function submitLogoutForm() {
            document.getElementById('logoutForm').submit();
            return false;
        }

        $(document).ready(function () {
            $('.toggle-order').click(function (e) {
                e.preventDefault(); // Ngăn chặn hành vi mặc định của liên kết

                // Đóng tất cả các submenu khác
                $('#mobile-navigation .submenu').slideUp(300).css("opacity", "1");

                // Hiện/ẩn danh sách con của mục hiện tại
                $(this).siblings('.submenu').slideToggle(300).css("opacity", "0").animate({ opacity: 1 }, 300);
            });

            // Đóng submenu khi nhấn ra ngoài menu
            $(document).click(function (e) {
                if (!$(e.target).closest('.mobile-menu').length) {
                    $('#mobile-navigation .submenu').slideUp(300).css("opacity", "1");
                }
            });
        });

 
        document.querySelector('.icon_review').addEventListener('click', function () {
            this.querySelector('.submenu').style.display =
                this.querySelector('.submenu').style.display === 'none' ? 'block' : 'none';
            this.querySelector('.toggle-icon').style.transform =
                this.querySelector('.submenu').style.display === 'none' ? 'rotate(0deg)' : 'rotate(180deg)';
        });

    


    
        document.querySelector('.toggle-order').addEventListener('click', function (e) {
            e.preventDefault();
            var submenu = this.nextElementSibling;
            var icon = this.querySelector('.toggle-icon');

            submenu.style.display = submenu.style.display === 'none' ? 'block' : 'none';
            icon.style.transform = submenu.style.display === 'none' ? 'rotate(0deg)' : 'rotate(180deg)';
        });
