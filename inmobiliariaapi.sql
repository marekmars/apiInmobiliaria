-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 01-11-2023 a las 01:56:01
-- Versión del servidor: 10.4.25-MariaDB
-- Versión de PHP: 8.1.10

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliariaapi`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `id` int(11) NOT NULL,
  `fechaInicio` datetime NOT NULL,
  `fechaFin` datetime NOT NULL,
  `estado` tinyint(1) NOT NULL,
  `mensualidad` double NOT NULL,
  `inmuebleId` int(11) NOT NULL,
  `inquilinoId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`id`, `fechaInicio`, `fechaFin`, `estado`, `mensualidad`, `inmuebleId`, `inquilinoId`) VALUES
(4, '2023-10-10 00:00:00', '2025-10-11 00:00:00', 1, 1500000, 1, 1),
(6, '2023-10-10 00:00:00', '2025-10-14 00:00:00', 1, 125000, 3, 5);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `id` int(11) NOT NULL,
  `lat` varchar(50) NOT NULL,
  `lon` varchar(50) NOT NULL,
  `uso` int(11) NOT NULL,
  `tipo` int(11) NOT NULL,
  `ambientes` int(11) NOT NULL,
  `disponible` tinyint(1) NOT NULL,
  `direccion` varchar(50) NOT NULL,
  `precio` double NOT NULL,
  `foto` varchar(250) NOT NULL,
  `propietarioId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`id`, `lat`, `lon`, `uso`, `tipo`, `ambientes`, `disponible`, `direccion`, `precio`, `foto`, `propietarioId`) VALUES
(1, '-32.35224722357396', '-64.97440296652641', 0, 1, 3, 0, 'Ruta 5 y La Usina', 150000, 'uploads/inmuebles/img_inmueble_1_1.jpg', 1),
(3, '-31.35224456465455', '-65.97440789789456', 1, 3, 3, 1, 'San Martin 123', 135000, 'uploads/inmuebles/img_inmueble_1_3.jpg', 1),
(35, '', '', 1, 0, 3, 1, 'Ruta 5 2345', 120000, 'uploads/inmuebles/img_inmueble_1_35.jpg', 1),
(36, '', '', 0, 0, 3, 1, 'calle el zorzal 123', 120000, 'uploads/inmuebles/img_inmueble_1_36.jpg', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `id` int(11) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `dni` varchar(50) NOT NULL,
  `telefono` varchar(50) NOT NULL,
  `correo` varchar(50) NOT NULL,
  `estado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `apellido`, `nombre`, `dni`, `telefono`, `correo`, `estado`) VALUES
(1, 'Rodriguez', 'Roxana', '12345678', '+54 9 11 1234 5678', 'roxi@mail.com', 1),
(4, 'Perez', 'Juan', '87654321', '+54 9 11 9876 5432', 'juan@mail.com', 1),
(5, 'Gonzalez', 'María', '55555555', '+54 9 11 5555 5555', 'maria@mail.com', 1),
(6, 'Lopez', 'Carlos', '99999999', '+54 9 11 9999 9999', 'carlos@mail.com', 1),
(7, 'Martinez', 'Laura', '11111111', '+54 9 11 1111 1111', 'laura@mail.com', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `id` int(11) NOT NULL,
  `nroPago` int(11) NOT NULL,
  `fechaPago` datetime NOT NULL,
  `importe` double NOT NULL,
  `idContrato` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`id`, `nroPago`, `fechaPago`, `importe`, `idContrato`) VALUES
(1, 1, '2023-10-14 15:27:08', 150000, 4),
(2, 2, '2023-10-16 15:27:42', 150000, 4),
(4, 1, '2023-10-26 19:52:10', 125000, 6);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `id` int(11) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `dni` varchar(50) NOT NULL,
  `telefono` varchar(50) NOT NULL,
  `correo` varchar(50) NOT NULL,
  `estado` tinyint(1) NOT NULL,
  `clave` varchar(250) NOT NULL,
  `avatar` varchar(250) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`id`, `apellido`, `nombre`, `dni`, `telefono`, `correo`, `estado`, `clave`, `avatar`) VALUES
(1, 'Silva', 'Nelson', '35830731', '2664690227', 'nelsonmarcosilva@gmail.com', 1, 'myl4T6FgkMUdldPQ96rZUnNYn0ho5fyVIc39WWFLd8Y=', 'uploads/avatars/avatar_1.png'),
(2, 'Rodriguez', 'Juan Manuel', '33123456', '2664236545', 'juan@mail.com', 1, 'myl4T6FgkMUdldPQ96rZUnNYn0ho5fyVIc39WWFLd8Y=', 'uploads/avatars/avatar_2.png');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `inmuebleId_pk` (`inmuebleId`),
  ADD KEY `inquilino_id` (`inquilinoId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `propietario_pk` (`propietarioId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `contratoId_pk` (`idContrato`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `inmuebleId_pk` FOREIGN KEY (`inmuebleId`) REFERENCES `inmuebles` (`id`),
  ADD CONSTRAINT `inquilino_id` FOREIGN KEY (`inquilinoId`) REFERENCES `inquilinos` (`id`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `propietario_pk` FOREIGN KEY (`propietarioId`) REFERENCES `propietarios` (`id`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `contratoId_pk` FOREIGN KEY (`idContrato`) REFERENCES `contratos` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
