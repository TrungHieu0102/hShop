using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "PictureUrl", "Slug" },
                values: new object[,]
                {
                    { new Guid("092b2f04-c372-4486-800b-ed25387f74fd"), "Tempore id provident sit placeat.", "Clothing", "https://picsum.photos/640/480/?image=438", "clothing" },
                    { new Guid("0f18c462-a8db-4866-8afe-f5e7ec234449"), "A voluptatibus deserunt recusandae ut.", "Toys", "https://picsum.photos/640/480/?image=600", "toys" },
                    { new Guid("101d0b00-7a81-429c-a123-9b4bfcfaf77d"), "Earum consequatur porro sunt similique.", "Computers", "https://picsum.photos/640/480/?image=327", "computers" },
                    { new Guid("1b0f7bdb-06b2-4bd4-84f8-c3dc9f07830f"), "Consequatur quo deserunt quia ut.", "Beauty", "https://picsum.photos/640/480/?image=779", "beauty" },
                    { new Guid("1c763a10-49ac-4aea-b880-9c1120c31a08"), "Quisquam aut laboriosam culpa blanditiis.", "Clothing", "https://picsum.photos/640/480/?image=398", "clothing" },
                    { new Guid("22b8cd1c-2116-48d1-86ad-946a979a74be"), "Quia impedit facere ea qui.", "Toys", "https://picsum.photos/640/480/?image=240", "toys" },
                    { new Guid("24d003bd-535a-418f-a975-5ca9ef353cfb"), "Maiores necessitatibus minus culpa ex.", "Books", "https://picsum.photos/640/480/?image=506", "books" },
                    { new Guid("279322c6-eb4b-46e8-b2e6-0396d29ee384"), "Iste aut illo a quae.", "Tools", "https://picsum.photos/640/480/?image=491", "tools" },
                    { new Guid("2ad99874-95b1-444d-b600-b095e824420d"), "Nam tempora ducimus sed rem.", "Toys", "https://picsum.photos/640/480/?image=266", "toys" },
                    { new Guid("2b6bea56-381f-40b5-9741-664a80448fd5"), "Magnam voluptatem dolore veritatis officia.", "Sports", "https://picsum.photos/640/480/?image=734", "sports" },
                    { new Guid("2edbd264-d3ad-4d20-aa46-d59028a59197"), "Ex accusantium voluptas et occaecati.", "Toys", "https://picsum.photos/640/480/?image=173", "toys" },
                    { new Guid("32bc8b83-d905-41a3-8ab0-1961e41c345b"), "Sit optio doloribus consequatur distinctio.", "Games", "https://picsum.photos/640/480/?image=685", "games" },
                    { new Guid("4334652b-ec62-4364-b686-f0edd48d7cc9"), "Qui maxime quia dolor dolorem.", "Jewelery", "https://picsum.photos/640/480/?image=1010", "jewelery" },
                    { new Guid("45070b03-f23c-4c6b-9be4-db8805ef8e45"), "Distinctio laborum facilis velit nihil.", "Jewelery", "https://picsum.photos/640/480/?image=880", "jewelery" },
                    { new Guid("494858bb-9a95-4096-a1a8-6c5ba67d8fa7"), "Sed odio nobis minima aut.", "Games", "https://picsum.photos/640/480/?image=911", "games" },
                    { new Guid("4a4ebc57-0151-484b-be70-1e5dc3f68806"), "Vero porro animi est minus.", "Computers", "https://picsum.photos/640/480/?image=836", "computers" },
                    { new Guid("4d0f1bbe-e130-4081-a555-d53b2380c530"), "Et illum necessitatibus eius alias.", "Games", "https://picsum.photos/640/480/?image=1023", "games" },
                    { new Guid("52c0fd41-9633-4190-b7fd-0a170f81c77f"), "Quo necessitatibus rerum rerum ullam.", "Sports", "https://picsum.photos/640/480/?image=810", "sports" },
                    { new Guid("5b327f92-32ad-4241-994a-383c82536d4d"), "Consequatur ut dolore qui illum.", "Clothing", "https://picsum.photos/640/480/?image=572", "clothing" },
                    { new Guid("5b4991fe-fb40-4977-8a14-845e5462d0cc"), "Veritatis sint earum nihil ea.", "Books", "https://picsum.photos/640/480/?image=472", "books" },
                    { new Guid("6e734e3f-7038-4d0e-aaf0-cb807d92ba3f"), "Quis eveniet eum velit numquam.", "Tools", "https://picsum.photos/640/480/?image=726", "tools" },
                    { new Guid("760972f9-fde3-495b-b757-cd87a0047d47"), "Error nihil quod quasi architecto.", "Automotive", "https://picsum.photos/640/480/?image=526", "automotive" },
                    { new Guid("7a04bb47-1b3d-4519-982d-b14d0d351750"), "Laborum ad accusantium dolorum minus.", "Sports", "https://picsum.photos/640/480/?image=612", "sports" },
                    { new Guid("7b755e97-d232-4798-95c0-f1816b50f1da"), "Aliquid qui quia harum iure.", "Clothing", "https://picsum.photos/640/480/?image=370", "clothing" },
                    { new Guid("7cae1b06-f23a-4310-97e5-0c3fe8876464"), "Blanditiis aspernatur odio rem deserunt.", "Grocery", "https://picsum.photos/640/480/?image=87", "grocery" },
                    { new Guid("7d841ef7-6c21-490f-996d-44eac28866ff"), "In consequatur modi tenetur consequatur.", "Books", "https://picsum.photos/640/480/?image=256", "books" },
                    { new Guid("82c66893-29a5-441d-9949-8b4a560714e4"), "Aliquid totam tempore ab quibusdam.", "Outdoors", "https://picsum.photos/640/480/?image=757", "outdoors" },
                    { new Guid("85c8a889-f731-4387-b1db-12fedd7ad980"), "Recusandae laboriosam quas amet fuga.", "Industrial", "https://picsum.photos/640/480/?image=200", "industrial" },
                    { new Guid("8c2b742c-1230-4fab-a1e6-a2048118cca9"), "Suscipit rerum ab nisi molestias.", "Jewelery", "https://picsum.photos/640/480/?image=823", "jewelery" },
                    { new Guid("94c3acee-3ce0-4814-afe6-3c0dac3d62e1"), "Consequatur beatae quidem fugit labore.", "Garden", "https://picsum.photos/640/480/?image=823", "garden" },
                    { new Guid("a6ff0571-0c12-4cf9-a821-14b4f0a75ff3"), "Voluptatibus aut voluptate non repudiandae.", "Tools", "https://picsum.photos/640/480/?image=715", "tools" },
                    { new Guid("b149843d-acd7-49e3-ad2f-dcf737854528"), "Nihil molestias nesciunt quasi quo.", "Electronics", "https://picsum.photos/640/480/?image=349", "electronics" },
                    { new Guid("b23cf752-591a-4bd7-90b2-eb9e420ea5c5"), "Natus earum occaecati ducimus fugiat.", "Jewelery", "https://picsum.photos/640/480/?image=483", "jewelery" },
                    { new Guid("bcaf1b09-508f-41bd-a25f-4b868bd1d1a5"), "Et beatae autem quod enim.", "Movies", "https://picsum.photos/640/480/?image=1028", "movies" },
                    { new Guid("be0573a5-7862-4a7d-9d23-5a58378edca6"), "Nemo dolorum ullam distinctio omnis.", "Toys", "https://picsum.photos/640/480/?image=357", "toys" },
                    { new Guid("c3234771-e8fb-4ae9-ac85-1e46cdb0c837"), "Laboriosam reprehenderit quaerat et nihil.", "Books", "https://picsum.photos/640/480/?image=731", "books" },
                    { new Guid("c51026f9-eaaf-40b9-a7a3-104b9e8bf9cd"), "Minima delectus ut voluptatum amet.", "Tools", "https://picsum.photos/640/480/?image=131", "tools" },
                    { new Guid("c818c5e8-f953-4c37-876e-11c72d744776"), "Consequatur voluptatem sit sint autem.", "Clothing", "https://picsum.photos/640/480/?image=921", "clothing" },
                    { new Guid("cccd69ba-5eb2-47a8-b99b-92cf6c4e2494"), "Accusamus vero dolore quisquam est.", "Games", "https://picsum.photos/640/480/?image=553", "games" },
                    { new Guid("d321eb70-ade3-43bb-b22e-0745381cd8ad"), "Natus cumque tempore perferendis quos.", "Baby", "https://picsum.photos/640/480/?image=403", "baby" },
                    { new Guid("d375d349-7e19-45e8-acb2-97ab292485cb"), "Odit accusantium non autem nostrum.", "Automotive", "https://picsum.photos/640/480/?image=242", "automotive" },
                    { new Guid("d509922f-f4f0-4cf4-863b-103341b4ac03"), "Odio rerum eius quo facilis.", "Outdoors", "https://picsum.photos/640/480/?image=730", "outdoors" },
                    { new Guid("d5fdbaf6-43d5-43b3-9766-2ad9d07a5c64"), "Et rem facere eligendi voluptatem.", "Beauty", "https://picsum.photos/640/480/?image=390", "beauty" },
                    { new Guid("d6334dae-1066-4739-84fa-47430f74a7e2"), "Recusandae amet qui sunt tempora.", "Health", "https://picsum.photos/640/480/?image=4", "health" },
                    { new Guid("d920a123-7f23-454b-8628-c0b871f4a049"), "Et ipsam eaque cumque veritatis.", "Tools", "https://picsum.photos/640/480/?image=814", "tools" },
                    { new Guid("e22049e0-d2bb-481d-ab5f-9ffec6b25b09"), "Incidunt minima provident iure qui.", "Grocery", "https://picsum.photos/640/480/?image=372", "grocery" },
                    { new Guid("e86830b1-f6cb-4242-9fdf-98481345c303"), "Est laborum delectus magnam velit.", "Clothing", "https://picsum.photos/640/480/?image=298", "clothing" },
                    { new Guid("ea735b72-db54-4355-bd29-33c11a1fc398"), "Maiores quo veritatis ut error.", "Garden", "https://picsum.photos/640/480/?image=517", "garden" },
                    { new Guid("ef55fb05-f737-4594-a814-c73b9ea59e19"), "Ipsam quis corporis culpa est.", "Home", "https://picsum.photos/640/480/?image=824", "home" },
                    { new Guid("fc614e1b-772c-4054-ba87-230f6c4eb5c4"), "Sit aut labore qui error.", "Sports", "https://picsum.photos/640/480/?image=523", "sports" }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "Description", "Email", "Logo", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { new Guid("01ab6e36-ab3c-4dea-a0ce-23545264aca6"), "101 Herzog Meadow, Pearlinetown, Qatar", "Quis aut illum vitae similique consequatur fuga nobis dolor nisi.", "Malachi72@gmail.com", "https://picsum.photos/640/480/?image=412", "Kemmer LLC", "0932874165" },
                    { new Guid("0f10c22a-6e09-46f4-8f86-d5b493b1a017"), "65604 Electa Harbor, Kuhictown, Romania", "Consequatur rem aut deleniti sit beatae doloribus minus velit nobis.", "Cody_Weimann69@yahoo.com", "https://picsum.photos/640/480/?image=440", "Keeling LLC", "0936810970" },
                    { new Guid("16df4e1e-e65f-47da-b532-1a6c942582e1"), "501 Crist Divide, Port Yolanda, Saint Martin", "At tempore quas occaecati quaerat nesciunt ad quidem laudantium natus.", "Deborah.Gorczany6@hotmail.com", "https://picsum.photos/640/480/?image=584", "Kassulke, Veum and Christiansen", "0916157366" },
                    { new Guid("192cc891-7804-4471-ad70-e73739f62256"), "74047 Howe Flats, South Randimouth, Tanzania", "Excepturi esse eaque ab debitis et distinctio perferendis possimus repellendus.", "Vivienne38@hotmail.com", "https://picsum.photos/640/480/?image=935", "Mraz - Crooks", "0999515393" },
                    { new Guid("1aab69ec-f826-4715-95e8-28e53a062304"), "70762 Harrison Estate, Micaelachester, Ireland", "Ab ea assumenda ut quos est aspernatur et deleniti fugiat.", "Daphne_Gleason@hotmail.com", "https://picsum.photos/640/480/?image=82", "Fisher - Heathcote", "0966915262" },
                    { new Guid("1fb8b9a9-e08a-4014-96c4-df86bab7ae91"), "121 Anahi Cove, North Reese, Democratic People's Republic of Korea", "Eius qui reprehenderit sunt vitae quasi labore accusamus vitae ipsum.", "Bernard.Ledner86@gmail.com", "https://picsum.photos/640/480/?image=877", "Frami, Waelchi and Brekke", "0979993802" },
                    { new Guid("23787776-63df-43e1-9a55-456312daf4fe"), "5123 Lucie Rest, Betsyborough, Jordan", "Officia repellendus commodi excepturi harum ipsa modi sit mollitia quidem.", "Anthony75@hotmail.com", "https://picsum.photos/640/480/?image=1058", "Morissette and Sons", "0925158796" },
                    { new Guid("26ae25a2-4fbd-41cc-95aa-d173fa5f0b11"), "4270 Cornell Lodge, North Kaci, Svalbard & Jan Mayen Islands", "Ducimus sed et in doloribus a ut est pariatur vero.", "Skye.Koss38@gmail.com", "https://picsum.photos/640/480/?image=179", "O'Hara - Gaylord", "0959457061" },
                    { new Guid("2a4ac1bc-dac3-4c76-8f51-818e933e0fb4"), "7199 Boehm Ways, Wehnerhaven, Guernsey", "Earum sit facere rerum ut laborum eos in perspiciatis ullam.", "Lloyd.Miller@yahoo.com", "https://picsum.photos/640/480/?image=943", "Schmidt - Koepp", "0945698899" },
                    { new Guid("31b17e45-8641-41d6-a59b-d2f67c965ec3"), "0553 Kshlerin Ridges, Shirleyview, Virgin Islands, U.S.", "Et inventore in aut quae voluptatem et omnis voluptas voluptate.", "Destiny.Ward47@gmail.com", "https://picsum.photos/640/480/?image=715", "Renner LLC", "0934487230" },
                    { new Guid("3217f699-6d8e-47d3-ad67-82fd0de12309"), "7287 Corwin Walk, Reingerchester, Azerbaijan", "Nam et pariatur dignissimos nulla et laudantium et excepturi sequi.", "Eva49@yahoo.com", "https://picsum.photos/640/480/?image=1051", "Gibson Group", "0924474230" },
                    { new Guid("34563da3-1b83-49e7-8ab8-81bd61b797dc"), "53869 Hickle Stravenue, Reubenview, Saint Pierre and Miquelon", "Perferendis tempora ut amet ut aliquid consequuntur aperiam atque at.", "Lexus63@hotmail.com", "https://picsum.photos/640/480/?image=585", "Ryan Group", "0957321869" },
                    { new Guid("365c5c5e-fb71-4379-bf05-060212ef21f4"), "99787 Quitzon Shore, New Mackenzie, Bouvet Island (Bouvetoya)", "Tenetur vel autem laudantium minus perspiciatis et fugit tempore ea.", "Kristy.Bosco79@gmail.com", "https://picsum.photos/640/480/?image=662", "Cremin - Kutch", "0974633345" },
                    { new Guid("4359fcaf-17aa-4d85-a441-5c5f031dd87e"), "7377 Blick Crossroad, Faheystad, Solomon Islands", "Ipsam doloribus velit voluptatem est et nemo exercitationem molestiae sed.", "Mckayla_Graham18@hotmail.com", "https://picsum.photos/640/480/?image=488", "Runolfsdottir LLC", "0953959300" },
                    { new Guid("43a237ae-2dd6-4e38-93a1-6106b9a834cc"), "5834 Ida Branch, Corwinburgh, Vanuatu", "Sequi nemo quia nostrum enim quis quia et natus vel.", "Jayde27@yahoo.com", "https://picsum.photos/640/480/?image=715", "Bogan - Kihn", "0983072538" },
                    { new Guid("4f670930-8c87-468d-b2f6-69ca297c3347"), "7957 Daugherty Turnpike, Port Isom, Venezuela", "Voluptas recusandae facere itaque nobis possimus inventore rem consectetur natus.", "Destinee83@yahoo.com", "https://picsum.photos/640/480/?image=183", "Klocko Group", "0959340665" },
                    { new Guid("54e8a3be-0627-4d51-a656-267a6e7e0ea9"), "1747 Mann Turnpike, Lake Colin, Gibraltar", "Officia fugiat id dolor adipisci aut quas cumque totam nihil.", "Felipa83@yahoo.com", "https://picsum.photos/640/480/?image=18", "Stamm LLC", "0991730852" },
                    { new Guid("551cdd5a-dbba-4de6-a7f8-4d53e57e893b"), "322 Fanny Branch, West Julianville, Lesotho", "Ducimus suscipit voluptatum blanditiis consequuntur dolores dicta qui perferendis culpa.", "Marquis16@gmail.com", "https://picsum.photos/640/480/?image=430", "Larson - Beer", "0983910411" },
                    { new Guid("581c3271-c341-4900-b794-a3c1986b342a"), "8024 Mayer Brook, South Tyresetown, Cambodia", "Expedita quo reiciendis ut veniam ut non laudantium dolorum sunt.", "Thea44@hotmail.com", "https://picsum.photos/640/480/?image=58", "Willms LLC", "0937277577" },
                    { new Guid("5b389060-58b5-4be6-bdd8-0894a6a099fa"), "1794 Cordell Streets, Sawaynville, Ethiopia", "Dolores totam ex sit quod vero distinctio et ut ipsum.", "Reymundo.Kub30@yahoo.com", "https://picsum.photos/640/480/?image=626", "Jenkins, Crooks and Harvey", "0974574998" },
                    { new Guid("61a39879-0740-4218-8753-51909d2acdf6"), "0546 Angelica Corners, Melynahaven, France", "Nihil facilis accusamus in rem id eos quo et similique.", "Laila.Dickens@yahoo.com", "https://picsum.photos/640/480/?image=292", "Flatley - Pfannerstill", "0948697815" },
                    { new Guid("632f239c-b3e2-4b9b-92e7-fec25794a865"), "66106 Farrell Summit, North Orvalport, Kiribati", "Praesentium enim enim quas et aut adipisci voluptatem qui ipsum.", "Dariana.Stanton79@yahoo.com", "https://picsum.photos/640/480/?image=78", "Conn, Mills and Parker", "0986901943" },
                    { new Guid("634972ed-cd93-4e41-883b-f3a5aba33015"), "539 Hessel Roads, Lake Neha, Belgium", "Velit et adipisci fuga odio quos magnam nesciunt omnis at.", "Marlene34@hotmail.com", "https://picsum.photos/640/480/?image=176", "Barrows, Crist and Crist", "0965323186" },
                    { new Guid("63ab1917-26f2-4568-ada3-0af2d42d112d"), "1717 Frieda Crescent, Bernierstad, Colombia", "Tenetur nostrum voluptates qui debitis laborum cupiditate nulla repudiandae itaque.", "Wendell_Sporer89@hotmail.com", "https://picsum.photos/640/480/?image=244", "Bechtelar and Sons", "0985298339" },
                    { new Guid("6874dfb0-6362-45c4-b319-1542c5b7d7c3"), "81589 Hilpert Knolls, Welchfurt, Syrian Arab Republic", "Odio ad odit voluptatibus ab velit reprehenderit distinctio non quis.", "Noemie.Ferry40@hotmail.com", "https://picsum.photos/640/480/?image=1066", "Sauer, Kub and Mills", "0973046843" },
                    { new Guid("7046a16c-8b84-4763-8886-03394853fd78"), "750 Zulauf Throughway, Lake Brennanstad, Andorra", "Et magni quia hic omnis eveniet placeat repellendus quaerat eveniet.", "Garth80@gmail.com", "https://picsum.photos/640/480/?image=18", "Hamill - Ankunding", "0991587763" },
                    { new Guid("760e1acc-5532-47eb-b13c-0532c25ea486"), "08387 Spinka Square, New Aimeeshire, Malta", "Enim sed aut eligendi voluptatem sapiente quis rem dolorem molestiae.", "Polly22@yahoo.com", "https://picsum.photos/640/480/?image=996", "Ferry - Bergstrom", "0931387089" },
                    { new Guid("769a9e05-bd38-4569-a490-e7325cdb7ab7"), "193 Kuphal Greens, Lempishire, Samoa", "Velit aut aut sapiente quo architecto eum nesciunt debitis laudantium.", "Dario25@gmail.com", "https://picsum.photos/640/480/?image=445", "Reynolds and Sons", "0911104184" },
                    { new Guid("811de89b-81d0-47ee-8e0a-011ba65d2406"), "711 Wunsch Road, Lednermouth, France", "Velit maxime quibusdam dolores est ullam placeat eum nemo qui.", "Casandra.Schultz48@hotmail.com", "https://picsum.photos/640/480/?image=664", "Glover - Labadie", "0999599257" },
                    { new Guid("84788cb3-117a-4033-82e4-634af22688af"), "339 Hand Mountain, Port Jamey, Cameroon", "Ad quo distinctio eveniet dolores repudiandae voluptatem qui nulla voluptatem.", "Edythe_Koepp72@gmail.com", "https://picsum.photos/640/480/?image=14", "Nader, Medhurst and Feest", "0943594555" },
                    { new Guid("8c8ef6ac-d24b-413a-b340-8306f31251b5"), "773 Florian Pike, East Camyllestad, Botswana", "Omnis eveniet voluptates animi et harum atque laudantium vitae magni.", "Ardith37@hotmail.com", "https://picsum.photos/640/480/?image=1050", "Bernier Inc", "0981483378" },
                    { new Guid("8d34aa57-adbf-4baf-86e7-c22783d16039"), "29274 Aryanna Freeway, Jaydenland, Antigua and Barbuda", "Hic facere qui quis dicta tempora et vitae sequi qui.", "Carissa53@yahoo.com", "https://picsum.photos/640/480/?image=560", "Maggio, Schiller and Conroy", "0945754671" },
                    { new Guid("8d7b0e8f-91d1-42e9-997d-4ac849c2f5f8"), "836 Weimann Mills, South Daniela, Sierra Leone", "In dolores omnis et placeat ipsum et enim eos doloribus.", "Jared.Ferry35@hotmail.com", "https://picsum.photos/640/480/?image=803", "White - Hilll", "0978197890" },
                    { new Guid("95994515-f2d3-45a1-a069-9ef2d7284f10"), "947 Schroeder Mount, South Elysechester, Morocco", "Neque qui et doloremque aut quo porro repellendus ducimus pariatur.", "Kaylin87@hotmail.com", "https://picsum.photos/640/480/?image=332", "Mayert, Schumm and Dickens", "0957456940" },
                    { new Guid("9c2e53f0-492d-43a9-9a83-55ce151e6d1a"), "715 Elmira Summit, Leliaside, French Southern Territories", "Temporibus temporibus ratione ex est dolor nesciunt perspiciatis aut modi.", "Lauretta68@gmail.com", "https://picsum.photos/640/480/?image=397", "D'Amore, Rath and Legros", "0964841451" },
                    { new Guid("a0bb5ac8-e0af-48a5-ae7b-06d59dd743b8"), "255 Nienow Bridge, West Price, Latvia", "Consequatur cumque voluptas dolor esse ut porro sit laboriosam deserunt.", "Jeramy.Gleichner@yahoo.com", "https://picsum.photos/640/480/?image=771", "Reilly - Crona", "0982790128" },
                    { new Guid("a570ede0-d788-490c-b09d-d6e53de533ad"), "60894 Goldner Forest, South Rogelio, Cuba", "Praesentium quae dicta ullam exercitationem facilis a consectetur et et.", "Torey.Bayer40@yahoo.com", "https://picsum.photos/640/480/?image=1066", "Steuber, Cartwright and Runte", "0939359217" },
                    { new Guid("b6a2a793-d515-4186-86ad-576812c29f96"), "4333 Wilderman Harbors, New Trey, Belarus", "Delectus nam et repellat nisi aut ea deserunt sequi fugit.", "Maeve91@hotmail.com", "https://picsum.photos/640/480/?image=1083", "Lehner Group", "0979655368" },
                    { new Guid("c1a84499-334d-44a3-8d2b-dd99b4559baf"), "4771 Orn Spurs, Port Theofort, Portugal", "Eius quam id corporis magni quasi non hic tempora eligendi.", "Winnifred_Reilly@gmail.com", "https://picsum.photos/640/480/?image=269", "Quitzon and Sons", "0977695046" },
                    { new Guid("c42efccd-e884-4fbc-924d-ee6e450bec0d"), "01423 Vandervort Mills, New Gildaland, Hungary", "A asperiores magni minus et nobis sed minima non qui.", "Melisa_Leuschke41@hotmail.com", "https://picsum.photos/640/480/?image=772", "Wyman LLC", "0959926054" },
                    { new Guid("c89cdfd5-b8e9-44bc-91b0-796518f85715"), "653 Nasir Falls, South Jaydechester, Brazil", "Facilis sunt libero reiciendis unde perferendis autem perferendis autem autem.", "Markus_Ledner@hotmail.com", "https://picsum.photos/640/480/?image=160", "Beer Group", "0947275534" },
                    { new Guid("cd92c3c2-9106-417c-a0df-8df25d43f631"), "272 Rebecca Brooks, Hellerchester, Kazakhstan", "Voluptatem sapiente expedita ab et voluptates corrupti ad sed ut.", "Stanley_Stehr79@gmail.com", "https://picsum.photos/640/480/?image=1009", "Cartwright and Sons", "0955853699" },
                    { new Guid("cedbc090-8e01-4ada-9448-8da5ba648f02"), "55242 Wilderman Drive, South Anamouth, Montenegro", "Quisquam quia voluptatem voluptatum ipsa dolor voluptatum consequatur animi ea.", "Arianna_Kilback80@gmail.com", "https://picsum.photos/640/480/?image=1007", "Langosh, McLaughlin and Olson", "0928057128" },
                    { new Guid("d4b72356-1e5e-4f8e-91f6-8ca9c0bdb938"), "387 Torp Spurs, Hicklehaven, Tokelau", "Voluptates neque quo aut voluptas quas corrupti rerum consequatur consequatur.", "Luisa_Ondricka@hotmail.com", "https://picsum.photos/640/480/?image=554", "Fadel - Franecki", "0915989149" },
                    { new Guid("d8e99e92-4889-48d8-b077-c9a35a067f0e"), "7669 Skiles Crossing, Pourosbury, Lithuania", "Exercitationem dicta fugit deleniti quasi totam omnis asperiores iusto quaerat.", "Thomas.Hammes79@hotmail.com", "https://picsum.photos/640/480/?image=926", "Shanahan Group", "0978229441" },
                    { new Guid("deb423e7-5a4f-4e18-aaf6-71eb63eda0b9"), "893 Bashirian Spring, South Alexieshire, Saint Pierre and Miquelon", "In deserunt rerum sit odit optio tenetur beatae accusamus architecto.", "Talon80@gmail.com", "https://picsum.photos/640/480/?image=566", "O'Conner, Rutherford and Berge", "0965570583" },
                    { new Guid("ded93312-ff02-4edb-82e9-b5ae9d8a0890"), "84130 Altenwerth Pass, East Travonburgh, Barbados", "Officiis quia praesentium sit earum nobis aut quis voluptatum rem.", "River.Grady@yahoo.com", "https://picsum.photos/640/480/?image=142", "Quigley - Frami", "0923346349" },
                    { new Guid("e9e08747-9496-42d9-8ed5-b821cdc7621b"), "98281 Cyrus Mountain, East Ryan, Anguilla", "Id voluptate et tempore repellat natus corporis qui et voluptatum.", "Eugenia.Green72@hotmail.com", "https://picsum.photos/640/480/?image=897", "Considine Inc", "0999309321" },
                    { new Guid("eb9e31ca-e0a5-45a5-8aa4-96703064d33c"), "082 Sipes Squares, Kyleeborough, Belgium", "Maiores laboriosam aut quia eveniet iste repellat saepe eligendi reiciendis.", "Kaleigh.Altenwerth66@yahoo.com", "https://picsum.photos/640/480/?image=593", "Hessel - O'Reilly", "0982651411" },
                    { new Guid("f68f524a-6426-417d-803c-bf73f6df7fa4"), "7095 Reece Gardens, South Avis, Finland", "Non et cupiditate velit velit architecto laborum quis et et.", "Glenna.OConner@yahoo.com", "https://picsum.photos/640/480/?image=997", "Ortiz and Sons", "0917937716" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("092b2f04-c372-4486-800b-ed25387f74fd"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("0f18c462-a8db-4866-8afe-f5e7ec234449"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("101d0b00-7a81-429c-a123-9b4bfcfaf77d"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("1b0f7bdb-06b2-4bd4-84f8-c3dc9f07830f"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("1c763a10-49ac-4aea-b880-9c1120c31a08"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22b8cd1c-2116-48d1-86ad-946a979a74be"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("24d003bd-535a-418f-a975-5ca9ef353cfb"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("279322c6-eb4b-46e8-b2e6-0396d29ee384"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("2ad99874-95b1-444d-b600-b095e824420d"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("2b6bea56-381f-40b5-9741-664a80448fd5"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("2edbd264-d3ad-4d20-aa46-d59028a59197"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("32bc8b83-d905-41a3-8ab0-1961e41c345b"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("4334652b-ec62-4364-b686-f0edd48d7cc9"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("45070b03-f23c-4c6b-9be4-db8805ef8e45"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("494858bb-9a95-4096-a1a8-6c5ba67d8fa7"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("4a4ebc57-0151-484b-be70-1e5dc3f68806"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("4d0f1bbe-e130-4081-a555-d53b2380c530"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("52c0fd41-9633-4190-b7fd-0a170f81c77f"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("5b327f92-32ad-4241-994a-383c82536d4d"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("5b4991fe-fb40-4977-8a14-845e5462d0cc"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("6e734e3f-7038-4d0e-aaf0-cb807d92ba3f"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("760972f9-fde3-495b-b757-cd87a0047d47"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("7a04bb47-1b3d-4519-982d-b14d0d351750"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("7b755e97-d232-4798-95c0-f1816b50f1da"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("7cae1b06-f23a-4310-97e5-0c3fe8876464"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("7d841ef7-6c21-490f-996d-44eac28866ff"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("82c66893-29a5-441d-9949-8b4a560714e4"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("85c8a889-f731-4387-b1db-12fedd7ad980"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("8c2b742c-1230-4fab-a1e6-a2048118cca9"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("94c3acee-3ce0-4814-afe6-3c0dac3d62e1"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a6ff0571-0c12-4cf9-a821-14b4f0a75ff3"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b149843d-acd7-49e3-ad2f-dcf737854528"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b23cf752-591a-4bd7-90b2-eb9e420ea5c5"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("bcaf1b09-508f-41bd-a25f-4b868bd1d1a5"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("be0573a5-7862-4a7d-9d23-5a58378edca6"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c3234771-e8fb-4ae9-ac85-1e46cdb0c837"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c51026f9-eaaf-40b9-a7a3-104b9e8bf9cd"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c818c5e8-f953-4c37-876e-11c72d744776"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("cccd69ba-5eb2-47a8-b99b-92cf6c4e2494"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d321eb70-ade3-43bb-b22e-0745381cd8ad"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d375d349-7e19-45e8-acb2-97ab292485cb"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d509922f-f4f0-4cf4-863b-103341b4ac03"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d5fdbaf6-43d5-43b3-9766-2ad9d07a5c64"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d6334dae-1066-4739-84fa-47430f74a7e2"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d920a123-7f23-454b-8628-c0b871f4a049"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("e22049e0-d2bb-481d-ab5f-9ffec6b25b09"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("e86830b1-f6cb-4242-9fdf-98481345c303"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("ea735b72-db54-4355-bd29-33c11a1fc398"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("ef55fb05-f737-4594-a814-c73b9ea59e19"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fc614e1b-772c-4054-ba87-230f6c4eb5c4"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("01ab6e36-ab3c-4dea-a0ce-23545264aca6"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("0f10c22a-6e09-46f4-8f86-d5b493b1a017"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("16df4e1e-e65f-47da-b532-1a6c942582e1"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("192cc891-7804-4471-ad70-e73739f62256"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("1aab69ec-f826-4715-95e8-28e53a062304"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("1fb8b9a9-e08a-4014-96c4-df86bab7ae91"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("23787776-63df-43e1-9a55-456312daf4fe"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("26ae25a2-4fbd-41cc-95aa-d173fa5f0b11"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("2a4ac1bc-dac3-4c76-8f51-818e933e0fb4"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("31b17e45-8641-41d6-a59b-d2f67c965ec3"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("3217f699-6d8e-47d3-ad67-82fd0de12309"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("34563da3-1b83-49e7-8ab8-81bd61b797dc"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("365c5c5e-fb71-4379-bf05-060212ef21f4"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("4359fcaf-17aa-4d85-a441-5c5f031dd87e"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("43a237ae-2dd6-4e38-93a1-6106b9a834cc"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("4f670930-8c87-468d-b2f6-69ca297c3347"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("54e8a3be-0627-4d51-a656-267a6e7e0ea9"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("551cdd5a-dbba-4de6-a7f8-4d53e57e893b"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("581c3271-c341-4900-b794-a3c1986b342a"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("5b389060-58b5-4be6-bdd8-0894a6a099fa"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("61a39879-0740-4218-8753-51909d2acdf6"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("632f239c-b3e2-4b9b-92e7-fec25794a865"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("634972ed-cd93-4e41-883b-f3a5aba33015"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("63ab1917-26f2-4568-ada3-0af2d42d112d"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("6874dfb0-6362-45c4-b319-1542c5b7d7c3"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("7046a16c-8b84-4763-8886-03394853fd78"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("760e1acc-5532-47eb-b13c-0532c25ea486"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("769a9e05-bd38-4569-a490-e7325cdb7ab7"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("811de89b-81d0-47ee-8e0a-011ba65d2406"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("84788cb3-117a-4033-82e4-634af22688af"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("8c8ef6ac-d24b-413a-b340-8306f31251b5"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("8d34aa57-adbf-4baf-86e7-c22783d16039"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("8d7b0e8f-91d1-42e9-997d-4ac849c2f5f8"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("95994515-f2d3-45a1-a069-9ef2d7284f10"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("9c2e53f0-492d-43a9-9a83-55ce151e6d1a"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("a0bb5ac8-e0af-48a5-ae7b-06d59dd743b8"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("a570ede0-d788-490c-b09d-d6e53de533ad"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("b6a2a793-d515-4186-86ad-576812c29f96"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("c1a84499-334d-44a3-8d2b-dd99b4559baf"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("c42efccd-e884-4fbc-924d-ee6e450bec0d"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("c89cdfd5-b8e9-44bc-91b0-796518f85715"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("cd92c3c2-9106-417c-a0df-8df25d43f631"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("cedbc090-8e01-4ada-9448-8da5ba648f02"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("d4b72356-1e5e-4f8e-91f6-8ca9c0bdb938"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("d8e99e92-4889-48d8-b077-c9a35a067f0e"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("deb423e7-5a4f-4e18-aaf6-71eb63eda0b9"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("ded93312-ff02-4edb-82e9-b5ae9d8a0890"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("e9e08747-9496-42d9-8ed5-b821cdc7621b"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("eb9e31ca-e0a5-45a5-8aa4-96703064d33c"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("f68f524a-6426-417d-803c-bf73f6df7fa4"));

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Products",
                type: "decimal(18,2",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
